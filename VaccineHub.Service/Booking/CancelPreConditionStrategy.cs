using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Models;

namespace VaccineHub.Service.Booking
{
    public class CancelPreConditionStrategy : IBookingPreConditionStrategy
    {
        private readonly IServiceProvider _serviceProvider;

        public CancelPreConditionStrategy(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Verify(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {            using var scope = _serviceProvider.CreateScope();
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
        
            // Ideally, it won't execute unless second vaccine dose can be taken within next 7 days interval
            // Check precondition
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