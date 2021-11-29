using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Concurrency;
using VaccineHub.Service.Types;
using VaccineHub.ThirdPartyService;
using VaccineHub.ThirdPartyService.Models;

namespace VaccineHub.Service.Booking
{
    public class BookOperationStrategy : IBookingOperationStrategy
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        private readonly IThirdPartyService _thirdPartyService;

        public BookOperationStrategy(IServiceProvider serviceProvider,
            IThirdPartyService thirdPartyService)
        {
            _serviceProvider = serviceProvider;
            _thirdPartyService = thirdPartyService;
        }

        public async Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            using (await AsyncSimulatedLock.LockAsync(booking.ProductId + booking.CenterId, cancellationToken))
            {
                var inventory = await dbContext.Inventories.Include(i => i.Product)
                    .Include(i => i.Center)
                    .FirstAsync(i => i.Product.Id == booking.ProductId && i.Center.Id == booking.CenterId,
                        cancellationToken);

                if (inventory.Stock == 0)
                {
                    throw new InvalidOperationException("Inventory out of stock");
                }

                var apiUser = await dbContext.ApiUsers.FindAsync(new object[] {apiUserId}, cancellationToken);
                var product = await dbContext.Products.FindAsync(new object[] {booking.ProductId}, cancellationToken);
                var center = await dbContext.Centers.FindAsync(new object[] {booking.CenterId}, cancellationToken);

                dbContext.Bookings.Add(new Persistence.Entities.Booking
                {
                    BookingType = Mapper.Map<Persistence.Types.BookingType>(booking.BookingType),
                    DosageType = Mapper.Map<Persistence.Types.DosageType>(booking.DosageType),
                    Product = product,
                    Center = center,
                    ApiUser = apiUser,
                    AppointmentDate = booking.AppointmentDate,
                    PaymentInformation =
                        Mapper.Map<Persistence.Entities.PaymentInformation>(booking.PaymentInformation)
                });

                try
                {
                    // Debit Payment from Customer Card
                    await _thirdPartyService.CallAsync(
                        new PaymentServiceRequest
                        {
                            TransactionType = TransactionType.Debit,
                            Cost = product.Cost,
                            PaymentInformation = 
                                Mapper.Map<PaymentInformation>(booking.PaymentInformation)
                        },
                        cancellationToken);
                }
                catch (Exception)
                {
                    throw new Exception("Debit transaction failed");
                }

                inventory.Stock -= 1;

                inventory.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Service.Models.PaymentInformation, PaymentInformation>();

                expression.CreateMap<Service.Models.PaymentInformation, Persistence.Entities.PaymentInformation>();

                expression.CreateMap<Types.BookingType, Persistence.Types.BookingType>();

                expression.CreateMap<DosageType, Persistence.Types.DosageType>();
            });
        }
    }
}