using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShoppingCart.Models
{
    public class AppUser : ShareModel
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public string EncryptedPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [NotMapped]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        // Navigation property for roles
        public List<AppRole> Roles { get; set; }

        // Navigation property for shopping cart
        public ShoppingCart ShoppingCart { get; set; }

        // Additional identification details
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
