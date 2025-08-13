namespace ECommerceApi.DTOs
{
    public class GetOrderDto
    {
        public int Id { get; set; }
        public List<GetOrderItemDto> Items { get; set; } = [];

        public decimal TotalAmount { get; set; } 
        public string? Currency { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? PaymentStatus { get; set; }
    }

}
