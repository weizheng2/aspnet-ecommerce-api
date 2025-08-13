using ECommerceApi.Models;

namespace ECommerceApi.DTOs
{
    public static class OrderMapper
    {
        public static GetOrderDto ToGetOrderDto(this Order order)
        {
            return new GetOrderDto
            {
                Id = order.Id,
                Items = order.Items.Select(o => o.ToGetOrderItemDto()).ToList(),
                TotalAmount = order.TotalAmount,
                Currency = order.Currency,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                PaymentStatus = order.PaymentStatus
            };
        }

        public static GetOrderItemDto ToGetOrderItemDto(this OrderItem orderItem)
        {
            return new GetOrderItemDto
            {
                ProductId = orderItem.ProductId,
                Product = orderItem.Product,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice
            };
        }
    }
}       