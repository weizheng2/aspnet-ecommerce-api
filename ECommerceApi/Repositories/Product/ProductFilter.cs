using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public class ProductFilter
    {
        public string? Name { get; init; }
        public ProductOrderBy OrderBy { get; init; } = ProductOrderBy.Price;
        public bool AscendingOrder { get; init; } = true;
    }
}