using MANS._55DR.HotelManagement.WebApi.Consts;
using MANS._55DR.HotelManagement.WebApi.Database;
using MANS._55DR.HotelManagement.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MANS._55DR.HotelManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.Admin)]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly HotelDbContext _context;

        public UsersController(UserManager<User> userManager, HotelDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var users = await _userManager.Users.Select(u => new UserModel
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName,
                IsAdmin = false,
                IsUser = false
            }).ToListAsync();

            var roles = await _context.Roles.ToListAsync();
            var userRoles = await _context.UserRoles.ToListAsync();

            var adminRoleId = roles.FirstOrDefault(r => r.Name == UserRoles.Admin)?.Id;
            var userRoleId = roles.FirstOrDefault(r => r.Name == UserRoles.User)?.Id;

            foreach (var user in users)
            {
                user.IsUser = userRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == userRoleId);
                user.IsAdmin = userRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == adminRoleId);
            }

            return Ok(users);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return Ok();
        }

        [HttpPost("UpgradeToUser/{userId}")]
        public Task<ActionResult> UpgradeToUser(string userId)
        {
            return UpgradeTo(userId, UserRoles.User);
        }

        [HttpPost("UpgradeToAdmin/{userId}")]
        public Task<ActionResult> UpgradeToAdmin(string userId)
        {
            return UpgradeTo(userId, UserRoles.Admin);
        }

        private async Task<ActionResult> UpgradeTo(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var identityResult = await _userManager.AddToRoleAsync(user, role);
            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult);
            }
            return Ok();
        }
    }
}
