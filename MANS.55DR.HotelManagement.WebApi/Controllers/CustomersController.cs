
using MANS._55DR.HotelManagement.WebApi.Consts;
using MANS._55DR.HotelManagement.WebApi.Database;
using MANS._55DR.HotelManagement.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MANS._55DR.HotelManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {

        private HotelDbContext _context;
        public CustomersController(HotelDbContext context)
        {
            _context = context;
        }

        // GET: api/<CustomerController>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Customer>>> Get()
        {
            return Ok(await _context.Customers.ToListAsync());
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Customer?>> Get(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // POST api/<CustomerController>
        [HttpPost]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
        public async Task<ActionResult<Customer>> Post([FromBody] Customer value)
        {
            _context.Customers.Add(value);
            await _context.SaveChangesAsync();
            return Ok(value);
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer value)
        {
            value.Id = id;
            _context.Entry(value).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.User}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
