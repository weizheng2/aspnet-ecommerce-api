using System.ComponentModel.DataAnnotations;

namespace ECommerceApi.DTOs
{
    public class UserCredentialsDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
            ErrorMessage = "Password must be at least 8 characters with uppercase, lowercase, digit, and special character.")]
        public string? Password { get; set; }
    }
}