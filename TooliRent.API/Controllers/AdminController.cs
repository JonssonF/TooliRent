using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TooliRent.Application.Authentication;
using TooliRent.Domain.Identity;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        private readonly RoleManager<IdentityRole> _roles;

        public AdminController(UserManager<AppUser> users, RoleManager<IdentityRole> roles)
        {
            _users = users;
            _roles = roles;
        }

        [HttpPost("Promote")]
        public async Task<ActionResult> Promote([FromBody] PromoteUserRequest request)
        {
            var user = await _users.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return NotFound("User not found.");
            }
            if (!await _roles.RoleExistsAsync("Admin"))
            {
                return BadRequest("Role 'Admin' does not exist.");
            }
            if (!await _users.IsInRoleAsync(user, "Admin"))
            {
                var add = await _users.AddToRoleAsync(user, "Admin");
                if (!add.Succeeded)
                {
                    return BadRequest(new { Errors = add.Errors.Select(e => e.Description) });
                }
            }
            if (request.RemoveMemberRole && await _users.IsInRoleAsync(user, "Member"))
            {
                var rem = await _users.RemoveFromRoleAsync(user, "Member");
                if (!rem.Succeeded)
                {
                    return BadRequest(new { Errors = rem.Errors.Select(e => e.Description) });
                }
            }
            return Ok(new
            {
                message = $"Member with email {request.Email} promoted to Admin.",
                user.Id,
                user.FullName,
                roles = await _users.GetRolesAsync(user)
            });
        }

        

        // Inte bästa sättet att få med rollerna i en user lista då det blir flera anrop till DB, undersök andra sätt att lösa detta på.

        /*[HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = _users.Users.ToList();

            var result = new List<object>();
            foreach(var u in users)
            {
                var roles = await _users.GetRolesAsync(u);
                result.Add(new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    Roles = roles
                });
            }

            return Ok(result);
        }*/
    }
}
