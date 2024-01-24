namespace MANS._55DR.HotelManagement.WebApi.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsUser { get; set; }
    }
}
