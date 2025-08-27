using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public record FilterProductDto : PaginationDto
    {
        public string? Name { get; init; }
        
        public ProductOrderBy OrderBy { get; init; } = ProductOrderBy.Price;
        public bool AscendingOrder { get; init; } = true;
    }
}

