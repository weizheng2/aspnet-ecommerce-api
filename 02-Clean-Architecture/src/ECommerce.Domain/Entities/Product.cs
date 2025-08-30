using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Product
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string? ImageUrl { get; private set; }

    private Product(string name, string? description, decimal price, string? imageUrl)
    {
        Name = name;
        Description = description;
        Price = price;
        ImageUrl = imageUrl;
    }

    public static Result<Product> Create(string name, string? description, decimal price, string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) 
            return Result<Product>.Failure(ResultErrorType.BadRequest, "Name is required");
            
        if (price <= 0)
            return Result<Product>.Failure(ResultErrorType.BadRequest, "Price must be greater than 0");

        return Result<Product>.Success(new Product(name, description, price, imageUrl));
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ResultErrorType.BadRequest, "Name is required");

        Name = name;
        return Result.Success();
    }

}




// using System.ComponentModel.DataAnnotations;
// using Microsoft.EntityFrameworkCore;

// namespace ECommerceApi.Models
// {
//     public class Product
//     {
//         public int Id { get; set; }

//         [Required(ErrorMessage = "The {0} field is required.")]
//         [StringLength(20, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
//         public required string Sku { get; set; }

//         [Required(ErrorMessage = "The {0} field is required.")]
//         [StringLength(100, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
//         public required string Name { get; set; } = string.Empty;

//         [StringLength(150, ErrorMessage = "The {0} field must be a string with a maximum length of {1}.")]
//         public string? Description { get; set; }

//         [Range(0.01, 99999.99, ErrorMessage = "The {0} must be between {1} and {2}.")]
//         public decimal Price { get; set; }

//         [Unicode(false)]
//         [Url(ErrorMessage = "The {0} field must be a valid URL.")]
//         public string? ImageUrl { get; set; }
//     }

// }