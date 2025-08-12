using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public static class CartMapper
    {
        public static GetCartDto ToGetCartDto(this Cart cart)
        {
            return new GetCartDto
            {
                Items = cart.Items.Select(c => c.ToGetCartItemDto()).ToList()
            };
        }

        public static GetCartItemDto ToGetCartItemDto(this CartItem cartItem)
        {
            return new GetCartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity
            };
        }
    }

}