using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Users;
using TooliRent.Application.Users.DTOs;
using TooliRent.Domain.Identity;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.API.Controllers.AdminOnly
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminUserService _admin;

        public AdminController(IAdminUserService admin)
        {
            _admin = admin;
        }
        // Promote a user to admin
        [HttpPost("promote")]
        public async Task<IActionResult> Promote([FromBody] PromoteUserRequest request, CancellationToken ct = default)
        {
            var (ok, status, error, data) = await _admin.PromoteAsync(request, ct);
            if (!ok) return StatusCode((int)status, new { error });

            return StatusCode((int)status, new
            {
                message = $"User {data!.Email} promoted to Admin.",
                user = data
            });
        }
        // Demote an admin to user
        [HttpPost("demote")]
        public async Task<IActionResult> Demote([FromBody] DemoteUserRequest request, CancellationToken ct = default)
        {
            var (ok, status, error, data) = await _admin.DemoteAsync(request, ct);
            if (!ok) return StatusCode((int)status, new { error });

            return StatusCode((int)status, new
            {
                message = $"Admin {data!.Email} demoted.",
                user = data
            });
        }
        // Deactivate a user account
        [HttpPost("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser([FromRoute] string id, CancellationToken ct = default)
        {
            var (ok, status, error, data) = await _admin.DeactivateAsync(id, ct);
            if (!ok) return StatusCode((int)status, new { error });
            return StatusCode((int)status, new {user = data});
        }

        // Activate a user account
        [HttpPost("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser([FromRoute] string id, CancellationToken ct = default)
        {
            var (ok, status, error, data) = await _admin.ActivateAsync(id, ct);
            if (!ok) return StatusCode((int)status, new { error });
            return StatusCode((int)status, new { user = data });
        }
    }
}

