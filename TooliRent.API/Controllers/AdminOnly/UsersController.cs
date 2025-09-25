using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Users;
using TooliRent.Application.Users.DTOs;
using TooliRent.Domain.Common;

namespace TooliRent.API.Controllers.AdminOnly
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service) 
        {
            _service = service;
        }

        // Paginated list of users with optional search and role filter
        [HttpGet]
        public async Task<ActionResult<PagedResult<UserDto>>> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] string? role = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetUsersPageAsync(page, pageSize, search, role, cancellationToken);
            return Ok(result);
        }

        // Get all users, sorted by role, up to a maximum number
        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(
            [FromQuery] int max = 100,
            CancellationToken cancellationToken = default)
        {
            if (max < 1 || max > 100_000)
            {
                max = 100;
            }

            var result = await _service.GetAllUsersSortedByRoleAsync(max, cancellationToken);
            return Ok(result);
        }
    }
}
