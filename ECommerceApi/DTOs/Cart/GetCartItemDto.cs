namespace ECommerceApi.DTOs
{
    public class GetCartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}