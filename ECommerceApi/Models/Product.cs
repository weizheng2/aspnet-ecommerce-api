using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public required string Sku { get; set; }

        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
    }

}