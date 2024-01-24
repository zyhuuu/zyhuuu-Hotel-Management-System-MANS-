using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace MANS._55DR.HotelManagement.WebApi.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Password confirmation do not mismatch Password")]
        public string? ConfirmPassword { get; set; }
    }
}
