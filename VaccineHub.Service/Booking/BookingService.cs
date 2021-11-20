using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.PaymentService;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Concurrency;
using VaccineHub.Service.Models;

namespace VaccineHub.Service.Booking
{
    public class BookingService : IBookingService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        private readonly IPaymentService _paymentService;

        public BookingService(IServiceProvider serviceProvider,
            IPaymentService paymentService)
        {
            _serviceProvider = serviceProvider;
            _paymentService = paymentService;
        }

        public async Task<bool> MakeOrCancelBookingAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            return booking.BookingType switch
            {
                BookingType.Book => await MakeBookingAsync(apiUserId, booking, cancellationToken),
                BookingType.Cancel => await CancelBookingAsync(apiUserId, booking, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(booking.BookingType.ToString())
            };
        }

        private async Task<bool> MakeBookingAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            // Check if any booking exists for product First Dose
            var existingBookedDbBookingForProductFirstDose = await dbContext.Bookings.Include(i => i.Product)
                .Include(i => i.ApiUser)
                .FirstOrDefaultAsync(
                    i => i.ApiUser.EmailId == apiUserId &&
                         i.Product.Id == booking.ProductId &&
                         i.DosageType == Persistence.Types.DosageType.First &&
                         i.BookingType == Persistence.Types.BookingType.Book,
                    cancellationToken);

            if (booking.DosageType == DosageType.First && existingBookedDbBookingForProductFirstDose != null)
            {
                throw new InvalidOperationException("First Dose Booking already exists");
            }

            // Check if any booking exists for product second Dose
            var existingBookedDbBookingForProductSecondDose = await dbContext.Bookings
                .Include(i => i.Product)
                .Include(i => i.ApiUser)
                .FirstOrDefaultAsync(
                    i => i.ApiUser.EmailId == apiUserId &&
                         i.Product.Id == booking.ProductId &&
                         i.DosageType == Persistence.Types.DosageType.Second &&
                         i.BookingType == Persistence.Types.BookingType.Book,
                    cancellationToken);

            if (booking.DosageType == DosageType.Second)
            {
                if (existingBookedDbBookingForProductFirstDose == null)
                {
                    throw new InvalidOperationException("First Dose Booking does not exist for taking Second dose");
                }

                if (existingBookedDbBookingForProductSecondDose != null)
                {
                    throw new InvalidOperationException("Second Dose Booking already exists");
                }

                if ((booking.AppointmentDate - existingBookedDbBookingForProductFirstDose.AppointmentDate).TotalDays <
                    existingBookedDbBookingForProductFirstDose.Product.MinIntervalInDays)
                {
                    throw new InvalidOperationException(
                        $"Second Dose should be taken after {existingBookedDbBookingForProductFirstDose.Product.MinIntervalInDays} days");
                }

                if ((booking.AppointmentDate - existingBookedDbBookingForProductFirstDose.AppointmentDate).TotalDays >
                    existingBookedDbBookingForProductFirstDose.Product.MaxIntervalInDays)
                {
                    throw new InvalidOperationException(
                        $"Second Dose should be taken within {existingBookedDbBookingForProductFirstDose.Product.MaxIntervalInDays} days");
                }
            }

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

                // Debit Payment from Customer Card
                await _paymentService.DebitPaymentAsync(
                    Mapper.Map<PaymentService.Models.PaymentInformation>(booking.PaymentInformation),
                    cancellationToken);

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

        private async Task<bool> CancelBookingAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            // Check if booking exists for product
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

            if (existingBookedDbBookingForProduct == null)
            {
                throw new InvalidOperationException("Booking details does not match");
            }

            // Check if second dose booking exists for product
            var existingBookedDbBookingForProductSecondDose = await dbContext.Bookings
                .Include(i => i.Product)
                .Include(i => i.ApiUser)
                .FirstOrDefaultAsync(
                    i => i.ApiUser.EmailId == apiUserId &&
                         i.Product.Id == booking.ProductId &&
                         i.BookingType == Persistence.Types.BookingType.Book &&
                         i.DosageType == Persistence.Types.DosageType.Second,
                    cancellationToken);

            // And verify if someone is trying to Cancel first 
            if (booking.DosageType == DosageType.First && existingBookedDbBookingForProductSecondDose != null)
            {
                throw new InvalidOperationException("Cannot Cancel First Dose Booking After Booking for Second dose");
            }

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

                // Cancelled
                existingBookedDbBookingForProduct.BookingType = Persistence.Types.BookingType.Cancel;

                inventory.Stock += 1;

                inventory.UpdatedAt = DateTime.Now;

                await dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
        }

        public Task<List<Models.Booking>> GetAllBookingsAsync(string apiUserId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            return dbContext.Bookings.Include(i => i.Product)
                .Include(i => i.Center)
                .Include(i => i.ApiUser)
                .Include(i => i.PaymentInformation)
                .Where(i => i.ApiUser.EmailId == apiUserId)
                .Select(i => Mapper.Map<Models.Booking>(i))
                .ToListAsync(cancellationToken);
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