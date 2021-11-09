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

namespace VaccineHub.Web.Endpoints.Product
{
    public sealed class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        private static readonly IMapper Mapper = CreateMapConfiguration()
            .CreateMapper();

        public ProductController([NotNull] IProductService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        #region Create
 
        /// <summary>
        ///  Add Product to Vaccine Hub Database
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Product")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AddProductAsync([FromBody] Models.Product product, CancellationToken token)
        {
            try
            {
                await _productService.AddProductAsync(Mapper.Map<Service.Models.Product>(product), token);

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
        ///  Retrieve Product from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Product('{productId}')")]
        [ProducesResponseType(typeof(Models.Product), 200)]
        public async Task<IActionResult> GetProductAsync(string productId, CancellationToken token)
        {
            var product = await _productService.GetProductAsync(productId, token);

            return Ok(product);
        }

        /// <summary>
        ///  Retrieve all Products from Vaccine Hub database
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("Products")]
        [ProducesResponseType(typeof(IList<Models.Product>), 200)]
        public async Task<IActionResult> GetProductsAsync(CancellationToken token)
        {
            var products = (await _productService.GetAllProductsAsync(token)).Select(product => Mapper.Map<Models.Product>(product));

            return Ok(products);
        }

        #endregion

        #region Update

        /// <summary>
        ///  Update Product from Vaccine Hub Database
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("Product('{productId}')")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateProductAsync(string productId, [FromBody] Models.Product product, CancellationToken token)
        {
            try
            {
                await _productService.UpdateProductAsync(productId, Mapper.Map<Service.Models.Product>(product), token);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        #endregion

        private static MapperConfiguration CreateMapConfiguration()
        {
            return new MapperConfiguration(expression =>
            {
                expression.CreateMap<Service.Models.Product, Models.Product>()
                    .ReverseMap();
            });
        }
    }
}