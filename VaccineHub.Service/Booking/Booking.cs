using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.PaymentService;
using VaccineHub.Persistence;
using VaccineHub.Service.Concurrency;
using VaccineHub.Service.Models;

namespace VaccineHub.Service.Booking
{
    public class Booking : IBooking
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        private readonly IPaymentService _paymentService;

        public Booking(IServiceProvider serviceProvider,
            IPaymentService paymentService)
        {
            _serviceProvider = serviceProvider;
            _paymentService = paymentService;
        }

        public async Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            return booking.BookingType switch
            {
                BookingType.Book => await PerformBookOperationAsync(apiUserId, booking, cancellationToken),
                BookingType.Cancel => await PerformCancelOperationAsync(apiUserId, booking, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(booking.BookingType?.ToString())
            };
        }

        private async Task<bool> PerformBookOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
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

                inventory.Stock -= 1;

                inventory.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }

        private async Task<bool> PerformCancelOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            using (await AsyncSimulatedLock.LockAsync(booking.ProductId + booking.CenterId, cancellationToken))
            {
                var inventory = await dbContext.Inventories.Include(i => i.Product)
                    .Include(i => i.Center)
                    .FirstAsync(i => i.Product.Id == booking.ProductId && i.Center.Id == booking.CenterId,
                        cancellationToken);

                // Credit Payment back to Customer Card
                await _paymentService.CreditPaymentAsync(
                    Mapper.Map<PaymentService.Models.PaymentInformation>(booking.PaymentInformation),
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

                existingBookedDbBookingForProduct.BookingType = Persistence.Types.BookingType.Cancel;

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
                expression.CreateMap<PaymentService.Models.PaymentInformation, PaymentInformation>()
                    .ReverseMap();

                expression.CreateMap<Persistence.Entities.PaymentInformation, PaymentInformation>()
                    .ReverseMap();

                expression.CreateMap<Persistence.Entities.Booking, Models.Booking>()
                    .ForMember(dst => dst.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                    .ForMember(dst => dst.CenterId, opt => opt.MapFrom(src => src.Center.Id))
                    .ReverseMap();

                expression.CreateMap<Persistence.Entities.Product, Models.Product>()
                    .ReverseMap();

                expression.CreateMap<Persistence.Entities.Center, Models.Center>()
                    .ReverseMap();
            });
        }
    }
}