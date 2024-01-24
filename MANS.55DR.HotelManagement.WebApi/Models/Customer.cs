using System.ComponentModel.DataAnnotations;

namespace MANS._55DR.HotelManagement.WebApi.Models
{
    public class Customer : Entity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; }
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
