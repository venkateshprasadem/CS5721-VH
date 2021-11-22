using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Models;

namespace VaccineHub.Service.Booking
{
    public class BookingPreConditionFilter : IFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public BookingPreConditionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            return booking.BookingType switch
            {
                BookingType.Book => VerifyBook(apiUserId, booking, cancellationToken),
                BookingType.Cancel => VerifyCancel(apiUserId, booking, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(booking.BookingType?.ToString())
            };
        }

        private async Task VerifyBook(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

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
        }
        
        private async Task VerifyCancel(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
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
        }
    }
}