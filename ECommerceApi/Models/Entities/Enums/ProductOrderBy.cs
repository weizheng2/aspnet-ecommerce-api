using System.Text.Json.Serialization;

namespace ECommerceApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductOrderBy
    {
        Name, Price
    }
}
