using System;
using System.Threading;
using System.Threading.Tasks;
using VaccineHub.ThirdPartyService;
using VaccineHub.ThirdPartyService.Models;

namespace VaccineHub.Service.Booking
{
    internal sealed class BookingThirdPartyDecorator : IBooking
    {
        private readonly IBooking _booking;

        private readonly IThirdPartyService _thirdPartyService;

        internal BookingThirdPartyDecorator(IBooking booking, IThirdPartyService thirdPartyService)
        {
            _booking = booking;
            _thirdPartyService = thirdPartyService;
        }

        public async Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            var returnVal = await _booking.PerformOperationAsync(apiUserId, booking, cancellationToken);

            // Call the Third partyService Api of centers to notify the booking details
            await _thirdPartyService.CallAsync(new NotifyDetailsRequest
            {
                EmailId = apiUserId,
                ProductId = booking.ProductId,
                AppointmentDate = booking.AppointmentDate,
                BookingType = Enum.Parse<BookingType>(booking.BookingType?.ToString()!)
            }, cancellationToken);

            return returnVal;
        }
    }
}