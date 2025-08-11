using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public class GetCartDto
    {
        public List<CartItem> Items { get; set; } = [];
    }

}