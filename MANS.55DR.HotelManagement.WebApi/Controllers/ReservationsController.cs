using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MANS._55DR.HotelManagement.WebApi.Database;
using MANS._55DR.HotelManagement.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using MANS._55DR.HotelManagement.WebApi.Consts;

namespace MANS._55DR.HotelManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly HotelDbContext _context;

        public ReservationsController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Reservation>>> Get()
        {
            var query = _context.Reservations.Include(r => r.Customer).Include(r => r.Room);
            return Ok(await query.ToListAsync());
        }

        // GET: Reservations/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Reservation>> Get(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }


        // PUT: Reservations/4
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
        public async Task<ActionResult<Reservation>> Put(int id, [FromBody] Reservation reservation)
        {
            //validation logic
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room == null)
            {
                return BadRequest(new
                {
                    Message = $"Room with id={reservation.RoomId} not exists"
                });
            }

            var customer = await _context.Customers.FindAsync(reservation.CustomerId);
            if (customer == null)
            {
                return BadRequest(new
                {
                    Message = $"customer with id={reservation.CustomerId} not exists"
                });
            }

            var collidingReservations = await _context.Reservations.Where(r => r.Id != id
                && r.RoomId == reservation.RoomId
                && (
                    reservation.StartDate < r.StartDate && reservation.EndDate >= r.StartDate
                    || reservation.StartDate >= r.StartDate && reservation.EndDate <= r.EndDate
                    || reservation.StartDate < r.EndDate && reservation.EndDate > r.EndDate
                )).ToListAsync();

            if (collidingReservations.Count > 0)
            {
                return BadRequest(new
                {
                    Message = "Room is booked"
                });
            }

            var duration = reservation.EndDate.Date - reservation.StartDate.Date;
            reservation.TotalAmount = (decimal)duration.TotalDays * (room.Price ?? 0);

            reservation.Id = id;
            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        // POST: Reservations
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
        public async Task<ActionResult<Reservation>> Post([FromBody] Reservation reservation)
        {
            //validation logic
            var room = await _context.Rooms.FindAsync(reservation.RoomId);
            if (room == null)
            {
                return BadRequest(new
                {
                    Message = $"Room with id={reservation.RoomId} not exists"
                });
            }

            var customer = await _context.Customers.FindAsync(reservation.CustomerId);
            if (customer == null)
            {
                return BadRequest(new
                {
                    Message = $"customer with id={reservation.CustomerId} not exists"
                });
            }

            var collidingReservations = await _context.Reservations.Where(r => r.RoomId == reservation.RoomId
                && (
                    reservation.StartDate < r.StartDate && reservation.EndDate >= r.StartDate
                    || reservation.StartDate >= r.StartDate && reservation.EndDate <= r.EndDate
                    || reservation.StartDate < r.EndDate && reservation.EndDate > r.EndDate
                )).ToListAsync();

            if (collidingReservations.Count > 0)
            {
                return BadRequest(new
                {
                    Message = "Room is booked"
                });
            }

            var duration = reservation.EndDate.Date - reservation.StartDate.Date;
            reservation.TotalAmount = (decimal)duration.TotalDays * (room.Price ?? 0);

            await _context.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        // Delete: Reservations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
