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

namespace VaccineHub.Web.Endpoints.Center
{
    public sealed class CenterController : ControllerBase
    {
        private readonly ICenterService _centerService;

        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        public CenterController([NotNull] ICenterService centerService)
        {
            _centerService = centerService ?? throw new ArgumentNullException(nameof(centerService));
        }

        #region Create
 
        /// <summary>
        ///  Add Center to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Center")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddCenterAsync([FromBody] Models.Center center, CancellationToken token)
        {
            await _centerService.AddCenterAsync(Mapper.Map<Service.Models.Center>(center), token);

            return NoContent();
        }

        #endregion

        #region Read

        /// <summary>
        ///  Retrieve Center from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Center('{centerId}')")]
        [ProducesResponseType(typeof(Models.Center), 200)]
        public async Task<IActionResult> GetCenterAsync(string centerId, CancellationToken token)
        {
            var center = await _centerService.GetCenterAsync(centerId, token);

            return Ok(center);
        }

        /// <summary>
        ///  Retrieve all Centers from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Centers")]
        [ProducesResponseType(typeof(IList<Models.Center>), 200)]
        public async Task<IActionResult> GetCentersAsync(CancellationToken token)
        {
            var centers = (await _centerService.GetAllCentersAsync(token)).Select(center => Mapper.Map<Models.Center>(center));

            return Ok(centers);
        }

        #endregion

        #region Update

        /// <summary>
        ///  Update Center from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("Center('{centerId}')")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateCenterAsync(string centerId, [FromBody] Models.Center center, CancellationToken token)
        {
            try
            {
                await _centerService.UpdateCenterAsync(centerId, Mapper.Map<Service.Models.Center>(center), token);

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
                expression.CreateMap<Service.Models.Center, Models.Center>()
                    .ReverseMap();
            });
        }
    }
}