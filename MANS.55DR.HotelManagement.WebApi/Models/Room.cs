using System.ComponentModel.DataAnnotations;

namespace MANS._55DR.HotelManagement.WebApi.Models
{
    public class Room : Entity
    {
        /// <summary>
        /// Room number
        /// </summary>
        public override int Id { get ; set ; }

        public string Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price must be greated then 0")]
        public decimal? Price { get; set; }
    }
}
