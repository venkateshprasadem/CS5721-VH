using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using VaccineHub.Service.Abstractions;

namespace VaccineHub.Service.Booking
{
    public class BookingService : IBookingService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public BookingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<string> MakeBookingAsync(string apiUserId, Models.Booking booking, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Booking> GetBookingAsync(string apiUserId, string bookingId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<List<Models.Booking>> GetAllBookingsAsync(string apiUserId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task CancelBookingAsync(string apiUserId, Models.Booking booking, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Persistence.Entities.Booking, Models.Booking>()
                    .ReverseMap();
            });
        }
    }
}