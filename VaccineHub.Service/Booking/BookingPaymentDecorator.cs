using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VaccineHub.PaymentService;
using VaccineHub.Service.Models;

namespace VaccineHub.Service.Booking
{
    internal sealed class BookingPaymentDecorator : IBooking
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IBooking _booking;

        private readonly IPaymentService _paymentService;

        internal BookingPaymentDecorator(IBooking booking, IPaymentService paymentService)
        {
            _booking = booking;
            _paymentService = paymentService;
        }

        public async Task<bool> PerformOperationAsync(string apiUserId, Models.Booking booking, CancellationToken cancellationToken)
        {
            var returnVal = await _booking.PerformOperationAsync(apiUserId, booking, cancellationToken);

            switch (booking.BookingType)
            {
                case BookingType.Book:
                    // Debit Payment from Customer Card
                    await _paymentService.DebitPaymentAsync(
                        Mapper.Map<PaymentService.Models.PaymentInformation>(booking.PaymentInformation),
                        cancellationToken);
                    break;
                case BookingType.Cancel:
                    // Credit Payment to Customer Card
                    await _paymentService.CreditPaymentAsync(
                        Mapper.Map<PaymentService.Models.PaymentInformation>(booking.PaymentInformation),
                        cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(booking.BookingType?.ToString());
            }

            return returnVal;
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<PaymentService.Models.PaymentInformation, PaymentInformation>()
                    .ReverseMap();
            });
        }
    }
}