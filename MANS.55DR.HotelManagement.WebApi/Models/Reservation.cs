using System.ComponentModel.DataAnnotations;

namespace MANS._55DR.HotelManagement.WebApi.Models
{
    public class Reservation : Entity, IValidatableObject
    {
        [Required]
        public string Details { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal? TotalAmount { get; set; }

        [Required]
        public int? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        [Required]
        public int? RoomId { get; set; }
        public virtual Room? Room { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> res = new List<ValidationResult>();
            if (StartDate.Date < DateTime.Now.Date)
            {
                res.Add(new ValidationResult("StartDate cannot be past"));
            }
            if (EndDate < StartDate)
            {
                res.Add(new ValidationResult("EndDate must be greater than StartDate"));
            }
            return res;
        }
    }
}
