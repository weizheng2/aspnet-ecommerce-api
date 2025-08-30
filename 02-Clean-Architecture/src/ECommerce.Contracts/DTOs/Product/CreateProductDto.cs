using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.DTOs
{
    public class CreateProductDto
    {

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(20, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
        public required string Sku { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(100, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
        public required string Name { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
        public string? Description { get; set; }

        [Range(0.01, 99999.99, ErrorMessage = "The {0} must be between {1} and {2}.")]
        public decimal Price { get; set; }

        [Unicode(false)]
        [Url(ErrorMessage = "The {0} field must be a valid URL.")]
        public string? ImageUrl { get; set; }
    }
}