using ECommerce.Domain.Entities;
using ECommerce.Application.Products;

namespace ECommerce.Application.Common.Mappings;

public static class ProductMapper
{
    public static GetProductDto ToGetProductDto(this Product product)
    {
        return new GetProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImageUrl = product.ImageUrl
        };
    }
}