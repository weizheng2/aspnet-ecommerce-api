using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using MediatR;
using ECommerce.Contracts.Common;
using ECommerce.Application.Products;
using ECommerce.Application.Common;

namespace ECommerce.Api.Controllers;

[ApiVersion("1.0")]
[EnableRateLimiting(Constants.RateLimitGeneral)]
[ApiController, Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _mediator;
    public ProductsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] PaginationRequest paginationRequest)
    {
        var command = new GetAllProductsQuery(new PaginationDto(paginationRequest.Page, paginationRequest.RecordsPerPage));
        var result = await _mediator.Send(command);

        return Ok(result.Data);
    }

    // [HttpGet("filter")]
    // public async Task<ActionResult<PagedResult<GetProductDto>>> GetProductsWithFilter([FromQuery] FilterProductDto filterProductDto)
    // {
    //     var result = await _productService.GetProductsFilterAsync(filterProductDto);
    //     return Ok(result.Data);
    // }

    // [HttpGet("{id}")]
    // public async Task<ActionResult<GetProductDto>> GetProduct(int id)
    // {
    //     var result = await _productService.GetProductAsync(id);
    //     if (result.IsSuccess)
    //         return Ok(result.Data);

    //     return NotFound(result.ErrorMessage);
    // }

    // [HttpPost]
    // [Authorize(Policy = Constants.PolicyIsAdmin)]
    // public async Task<ActionResult> CreateProduct(CreateProductDto createProductDto)
    // {
    //     var result = await _productService.CreateProductAsync(createProductDto);
    //     if (result.IsSuccess)
    //     {
    //         var product = result.Data;
    //         return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    //     }

    //     switch (result.ErrorType)
    //     {
    //         case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
    //         default: return BadRequest(result.ErrorMessage);
    //     }
    // }

    // [HttpPut("{id}")]
    // [Authorize(Policy = Constants.PolicyIsAdmin)]
    // public async Task<ActionResult> UpdateProduct(int id, UpdateProductDto updateProductDto)
    // {
    //     var result = await _productService.UpdateProductAsync(id, updateProductDto);
    //     if (result.IsSuccess)
    //         return NoContent();

    //     switch (result.ErrorType)
    //     {
    //         case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
    //         default: return BadRequest(result.ErrorMessage);
    //     }
    // }

    // [HttpDelete("{id}")]
    // [Authorize(Policy = Constants.PolicyIsAdmin)]
    // public async Task<ActionResult> DeleteProduct(int id)
    // {
    //     var result = await _productService.DeleteProductAsync(id);
    //     if (result.IsSuccess)
    //         return NoContent();

    //     switch (result.ErrorType)
    //     {
    //         case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
    //         default: return BadRequest(result.ErrorMessage);
    //     }
    // }
}



