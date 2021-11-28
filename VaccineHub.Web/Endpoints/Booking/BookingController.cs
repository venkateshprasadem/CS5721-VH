using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VaccineHub.Service.Abstractions;
using VaccineHub.Service.Models;

namespace VaccineHub.Web.Endpoints.Booking
{
    public sealed class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        public BookingController([NotNull] IBookingService bookingService)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        }

        #region Create
 
        /// <summary>
        ///  Add Booking to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [Route("Booking")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> MakeOrCancelBookingAsync([FromBody] Models.Booking booking, CancellationToken token)
        {
            try
            {
                await _bookingService.MakeOrCancelBookingAsync(User.Identity?.Name, Mapper.Map<Service.Models.Booking>(booking),
                    token);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Read

        /// <summary>
        ///  Retrieve all the Bookings from Vaccine Hub database by Role
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Bookings")]
        [ProducesResponseType(typeof(IList<Models.Booking>), 200)]
        public async Task<IActionResult> GetAllBookingsAsync(CancellationToken token)
        {
            if (User?.Claims.FirstOrDefault(f => f.Value.Equals("Admin")) != null)
            {
                return Ok((await _bookingService.GetAllBookingsAsync(null, token)).Select(
                        booking => Mapper.Map<Models.Booking>(booking)));
            }
            return Ok((await _bookingService.GetAllBookingsAsync(User?.Identity?.Name, token)).Select(
                    booking => Mapper.Map<Models.Booking>(booking)));
        }

        /// <summary>
        ///  Retrieve all the Bookings from Vaccine Hub database by Customer Id
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Bookings('{customerId}')")]
        [ProducesResponseType(typeof(IList<Models.Booking>), 200)]
        public async Task<IActionResult> GetAllBookingsByCustomerIdAsync(string customerId, CancellationToken token)
        {
            var bookings =
                (await _bookingService.GetAllBookingsAsync(customerId, token)).Select(
                    booking => Mapper.Map<Models.Booking>(booking));

            return Ok(bookings);
        }

        #endregion

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<PaymentInformation, Models.PaymentInformation>()
                    .ReverseMap();

                expression.CreateMap<Service.Models.Booking, Models.Booking>()
                    .ReverseMap();
            });
        }
    }
}