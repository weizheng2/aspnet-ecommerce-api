using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public class GetCartDto
    {
        public List<GetCartItemDto> Items { get; set; } = [];
    }

}