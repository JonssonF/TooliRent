using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TooliRent.Application.Authentication;
using TooliRent.Domain.Entities;
using TooliRent.Infrastructure.Identity;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly ITokenService _tokens;
        private readonly AppDbContext _context;

        public AuthController(UserManager<AppUser> users, RoleManager<IdentityRole> roles, ITokenService tokens, AppDbContext context)
        {
            _users = users;
            _roles = roles;
            _tokens = tokens;
            _context = context;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            var user = new AppUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                FullName = request.FullName
            };

            var create = await _users.CreateAsync(user, request.Password);
            if (!create.Succeeded)
                return BadRequest(new { Errors = create.Errors.Select(e => e.Description) });

            if (await _roles.RoleExistsAsync("Member"))
            {
                await _users.AddToRoleAsync(user, "Member");
            }

            var (access, expires) = await _tokens.CreateAccessTokenAsync(user);
            var refresh = _tokens.CreateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refresh,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            });
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse(access, refresh, expires));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            var user = await _users.FindByEmailAsync(request.Email);
            if (user is null || !await _users.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("The credentials you have provided is not registered.");
            }

            var valid = await _users.CheckPasswordAsync(user, request.Password);
            if (!valid)
            {
                return Unauthorized("Bad credentials.");
            }

            var (access, expires) = await _tokens.CreateAccessTokenAsync(user);
            var refresh = _tokens.CreateRefreshToken();

            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = refresh,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            });
            await _context.SaveChangesAsync();
            return Ok(new AuthResponse(access, refresh, expires));
        }

        [HttpPost("Refresh")]
        [AllowAnonymous]

        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
        {
            var rt = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.Revoked);

            if (rt is null || rt.ExpiresUtc < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            var user = await _users.FindByIdAsync(rt.UserId);
            if (user is null)
            {
                return Unauthorized("The user associated with this refresh token could not be found.");
            }

            rt.Revoked = true;
            var newRefresh = _tokens.CreateRefreshToken();
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefresh,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            });

            var (access, expires) = await _tokens.CreateAccessTokenAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse(access, newRefresh, expires));
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<ActionResult<object>> Me()
        {
            var userId = User?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }

            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                return Unauthorized();
            }

            var roles = await _users.GetRolesAsync(user);
            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                Roles = roles
            });
        }
    }
}
