using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.DTOs
{
    public class UpdateProductDto
    {
        [StringLength(100, ErrorMessage = "The {0} field must be a maximum length of {1}.")]
        public string? Name { get; set; }

        [StringLength(150, ErrorMessage = "The {0} field must be a maximum length of {1}.")]
        public string? Description { get; set; }

        [Range(0.01, 99999.99, ErrorMessage = "The {0} must be between {1} and {2}.")]
        public decimal? Price { get; set; }

        [Url(ErrorMessage = "The {0} field must be a valid URL.")]
        public string? ImageUrl { get; set; }
    }
}