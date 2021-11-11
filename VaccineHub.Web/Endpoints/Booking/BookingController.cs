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
        public async Task<IActionResult> MakeBookingAsync([FromBody] Models.Booking booking, CancellationToken token)
        {
            await _bookingService.MakeBookingAsync(User.Identity?.Name, Mapper.Map<Service.Models.Booking>(booking),
                token);

            return NoContent();
        }

        #endregion

        #region Read

        /// <summary>
        ///  Retrieve all the Bookings from Vaccine Hub database by Role
        /// </summary>
        [HttpGet]
        [Authorize()]
        [Route("Bookings")]
        [ProducesResponseType(typeof(IList<Models.Booking>), 200)]
        public async Task<IActionResult> GetAllBookingsAsync(CancellationToken token)
        {
            var bookings =
                (await _bookingService.GetAllBookingsAsync(User.Identity?.Name, token)).Select(
                    booking => Mapper.Map<Models.Booking>(booking));

            return Ok(bookings);
        }

        #endregion

        #region Update

        /// <summary>
        ///  Cancel Booking from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Customer")]
        [Route("Booking")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> CancelBookingAsync([FromBody] Models.Booking booking, CancellationToken token)
        {
            try
            {
                await _bookingService.CancelBookingAsync(User.Identity?.Name, Mapper.Map<Service.Models.Booking>(booking), token);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Service.Models.Booking, Models.Booking>()
                    .ReverseMap();
            });
        }
    }
}