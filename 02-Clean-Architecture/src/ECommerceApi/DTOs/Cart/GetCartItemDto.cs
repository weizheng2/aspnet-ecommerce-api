using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public class GetCartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Product!.Price * Quantity;

    }

}