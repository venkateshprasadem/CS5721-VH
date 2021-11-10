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

namespace VaccineHub.Web.Endpoints.Inventory
{
    public sealed class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        public InventoryController([NotNull] IInventoryService inventoryService)
        {
            _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        }

        #region Create
 
        /// <summary>
        ///  Add inventory to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Inventory")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddInventoryAsync([FromBody] Models.Inventory inventory, CancellationToken token)
        {
            try
            {
                await _inventoryService.AddInventoryAsync(Mapper.Map<Service.Models.Inventory>(inventory), token);

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
        ///  Retrieve all inventories from Vaccine Hub database by centerId
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Inventories")]
        [ProducesResponseType(typeof(IList<Models.Inventory>), 200)]
        public async Task<IActionResult> GetAllInventoriesAsync(CancellationToken token)
        {
            var product =
                (await _inventoryService.GetAllInventoriesAsync(token)).Select(inventory =>
                    Mapper.Map<Models.Inventory>(inventory));

            return Ok(product);
        }

        /// <summary>
        ///  Retrieve inventories from Vaccine Hub database by productId
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Inventories(productId/'{productId}')")]
        [ProducesResponseType(typeof(IList<Models.Inventory>), 200)]
        public async Task<IActionResult> GetAllInventoriesByProductIdAsync(string productId, CancellationToken token)
        {
            try
            {
                var inventories =
                    (await _inventoryService.GetAllInventoriesByProductIdAsync(productId, token)).Select(inventory =>
                        Mapper.Map<Models.Inventory>(inventory));

                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  Retrieve all inventories from Vaccine Hub database by centerId
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Inventories('centerId/{centerId}')")]
        [ProducesResponseType(typeof(IList<Models.Inventory>), 200)]
        public async Task<IActionResult> GetAllInventoriesByCenterIdAsync(string centerId, CancellationToken token)
        {
            try
            {
                var inventories =
                    (await _inventoryService.GetAllInventoriesByCenterIdAsync(centerId, token)).Select(inventory =>
                        Mapper.Map<Models.Inventory>(inventory));

                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Update

        /// <summary>
        ///  Update inventory from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("Inventory")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateInventoryAsync([FromBody] Models.Inventory inventory, CancellationToken token)
        {
            try
            {
                await _inventoryService.UpdateInventoryAsync(Mapper.Map<Service.Models.Inventory>(inventory), token);

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
                expression.CreateMap<Service.Models.Inventory, Models.Inventory>()
                    .ReverseMap();
            });
        }
    }
}