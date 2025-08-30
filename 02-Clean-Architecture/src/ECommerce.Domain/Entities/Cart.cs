// using System.ComponentModel.DataAnnotations;

// namespace ECommerceApi.Models
// {
//     public class Cart
//     {
//         public int Id { get; set; }

//         [Required]
//         public required string UserId { get; set; }
//         public User? User { get; set; }

//         public List<CartItem> Items { get; set; } = [];
//         public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
        
//     }

// }