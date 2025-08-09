using System.Text.Json.Serialization;

namespace ECommerceApi.DTOs
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductOrderBy
    {
        Name, Price
    }

    public record FilterProductDto : PaginationDto
    {
        public string? Name { get; init; }
        
        public ProductOrderBy OrderBy { get; init; } = ProductOrderBy.Price;
        public bool AscendingOrder { get; init; } = true;

    }
}

