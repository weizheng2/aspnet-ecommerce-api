using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public static class ProductMapper
    {
        public static Product ToProduct(this CreateProductDto createProductDto)
        {
            return new Product
            {
                Sku = createProductDto.Sku,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                ImageUrl = createProductDto.ImageUrl
            };
        }
        
        public static GetProductDto ToGetProductDto(this Product product)
        {
            return new GetProductDto
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };
        }


    }

}