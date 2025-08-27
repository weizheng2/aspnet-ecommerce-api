using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public class GetCartDto
    {
        public List<GetCartItemDto> Items { get; set; } = [];
        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }

}