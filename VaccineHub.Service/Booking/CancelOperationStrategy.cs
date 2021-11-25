using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Concurrency;
using VaccineHub.ThirdPartyService;
using VaccineHub.ThirdPartyService.Models;

namespace VaccineHub.Service.Booking
{
    public class CancelOperationStrategy : IBookingOperationStrategy
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        private readonly IThirdPartyService _thirdPartyService;

        public CancelOperationStrategy(IServiceProvider serviceProvider,
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

                // Cancel
                var existingBookedDbBookingForProduct = await dbContext.Bookings
                    .Include(i => i.Product)
                    .Include(i => i.Center)
                    .Include(i => i.ApiUser)
                    .FirstOrDefaultAsync(
                        i => i.ApiUser.EmailId == apiUserId &&
                             i.Product.Id == booking.ProductId &&
                             i.Center.Id == booking.CenterId &&
                             i.BookingType == Persistence.Types.BookingType.Book &&
                             (int?) i.DosageType == (int?) booking.DosageType &&
                             DateTime.Compare(i.AppointmentDate, booking.AppointmentDate) == 0
                        ,
                        cancellationToken);

                // Credit Payment back to Customer Card
                await _thirdPartyService.CallAsync(
                    new PaymentServiceRequest
                    {
                        TransactionType = TransactionType.Credit,
                        Cost = existingBookedDbBookingForProduct.Product.Cost,
                        PaymentInformation = 
                            Mapper.Map<PaymentInformation>(booking.PaymentInformation)
                    },
                    cancellationToken);

                // Recording latest paymentInformation only
                existingBookedDbBookingForProduct.PaymentInformation = Mapper.Map<Persistence.Entities.PaymentInformation>(booking.PaymentInformation);
                existingBookedDbBookingForProduct.BookingType = Persistence.Types.BookingType.Cancel;
                existingBookedDbBookingForProduct.UpdatedAt = DateTime.Now;

                inventory.Stock += 1;
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
            });
        }
    }
}