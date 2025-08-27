using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using ECommerceApi.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [EnableRateLimiting(Constants.RateLimitGeneral)]
    [ControllerName("Products"), Tags("Products")]
    [ApiController, Route("api/v{version:apiVersion}/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<GetProductDto>>> GetAllProducts([FromQuery] PaginationDto paginationDto)
        {
            var result = await _productService.GetAllProductsAsync(paginationDto);
            return Ok(result.Data);
        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagedResult<GetProductDto>>> GetProductsWithFilter([FromQuery] FilterProductDto filterProductDto)
        {
            var result = await _productService.GetProductsFilterAsync(filterProductDto);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetProductDto>> GetProduct(int id)
        {
            var result = await _productService.GetProductAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize(Policy = Constants.PolicyIsAdmin)]
        public async Task<ActionResult> CreateProduct(CreateProductDto createProductDto)
        {
            var result = await _productService.CreateProductAsync(createProductDto);
            if (result.IsSuccess)
            {
                var product = result.Data;
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Constants.PolicyIsAdmin)]
        public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var result = await _productService.UpdateProductAsync(id, updateProductDto);
            if (result.IsSuccess)
                return NoContent();

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = Constants.PolicyIsAdmin)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result.IsSuccess)
                return NoContent();

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }
    }
}


