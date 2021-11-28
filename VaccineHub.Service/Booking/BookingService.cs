using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VaccineHub.Persistence;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Models;
using VaccineHub.ThirdPartyService;

namespace VaccineHub.Service.Booking
{
    internal class BookingService : IBookingService
    {
        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        private readonly IServiceProvider _serviceProvider;

        public BookingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> MakeOrCancelBookingAsync(string apiUserId, Models.Booking booking,
            CancellationToken cancellationToken)
        {
            FilterManager filterManager;

            switch (booking.BookingType)
            {
                case BookingType.Book:
                    filterManager = new FilterManager(new BookingThirdPartyDecorator(
                        new Booking(new BookOperationStrategy(_serviceProvider,
                            _serviceProvider.GetRequiredService<IThirdPartyService>())),
                        _serviceProvider.GetRequiredService<IThirdPartyService>()));
                    filterManager.SetFilter(
                        new BookingPreConditionFilter(new BookPreConditionStrategy(_serviceProvider)));
                    break;
                case BookingType.Cancel:
                    filterManager = new FilterManager(new BookingThirdPartyDecorator(
                        new Booking(new CancelOperationStrategy(_serviceProvider,
                            _serviceProvider.GetRequiredService<IThirdPartyService>())),
                        _serviceProvider.GetRequiredService<IThirdPartyService>()));
                    filterManager.SetFilter(
                        new BookingPreConditionFilter(new CancelPreConditionStrategy(_serviceProvider)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(booking.BookingType?.ToString());
            }

            await filterManager.FilterRequest(apiUserId, booking, cancellationToken);
            return true;
        }

        public Task<List<Models.Booking>> GetAllBookingsAsync(string apiUserId, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IVaccineHubDbContext>();

            if (apiUserId == null)
            {
                return dbContext.Bookings.Include(i => i.Product)
                    .Include(i => i.Center)
                    .Include(i => i.ApiUser)
                    .Include(i => i.PaymentInformation)
                    .Select(i => Mapper.Map<Models.Booking>(i))
                    .ToListAsync(cancellationToken);
            }

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