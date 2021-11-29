using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Models;
using VaccineHub.Service.Types;

namespace VaccineHub.Service.Booking
{
    public class BookPreConditionStrategy : IBookingPreConditionStrategy
    {
        private readonly IServiceProvider _serviceProvider;

        public BookPreConditionStrategy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
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
    }
}