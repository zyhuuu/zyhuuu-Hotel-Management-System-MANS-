using MANS._55DR.HotelManagement.WebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MANS._55DR.HotelManagement.WebApi.Database
{
    public class HotelDbContext : IdentityDbContext<User>
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public HotelDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
