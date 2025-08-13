using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }
        public User? User { get; set; }

        public List<OrderItem> Items { get; set; } = [];

        public decimal TotalAmount { get; set; } 
        public string? Currency { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; } 
        public string? PaymentToken { get; set; }
    }

}