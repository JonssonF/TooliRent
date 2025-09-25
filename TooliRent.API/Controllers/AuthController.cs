using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TooliRent.Application.Authentication;
using TooliRent.Application.Authentication.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Identity;
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


        /*------------Helper Method to find correct User throu claims----------------*/
        private string? GetUserIdFromClaims() =>
               User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        /*---------------------------------------------------------------------------*/
        // Register a new user and get access token + refresh token
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
            {
                return BadRequest(new { Errors = create.Errors.Select(e => e.Description) });
            }
            //Puts new user in "Member" role if it exists
            if (await _roles.RoleExistsAsync(Roles.Member))
            {
                await _users.AddToRoleAsync(user, Roles.Member);
            }

            var roles = await _users.GetRolesAsync(user);
            var authUser = new AuthUser
            (
                user.Id,
                user.Email,
                user.UserName,
                roles.ToArray()
            );
            var tokenResult = await _tokens.CreateAccessTokenAsync(authUser);
            var (access, expires) = await _tokens.CreateAccessTokenAsync(authUser);

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
        // Login and get access token + refresh token
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

            var roles = await _users.GetRolesAsync(user);
            var authUser = new AuthUser
            (
                user.Id,
                user.Email,
                user.UserName,
                roles.ToArray()
            );
            var (access, expires) = await _tokens.CreateAccessTokenAsync(authUser);

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
        // Refresh access token using refresh token
        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
        {
            // Initial accesstoken is only stored locally, only refresh token is stored in DB.

            // Find refresh token in DB, must exists and not revoked
            var rt = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.Revoked);

            if (rt is null || rt.ExpiresUtc < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            // Find user associated with this token
            var user = await _users.FindByIdAsync(rt.UserId);
            if (user is null)
            {
                return Unauthorized("The user associated with this refresh token could not be found.");
            }

            // Revoke current refresh token and create a new one (rotation)
            rt.Revoked = true;

            var newRefresh = _tokens.CreateRefreshToken();
            _context.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefresh,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            });

            // Create new access token for user
            var roles = await _users.GetRolesAsync(user);
            var authUser = new AuthUser
            (
                user.Id,
                user.Email,
                user.UserName,
                roles.ToArray()
            );
            var tokenResult = await _tokens.CreateAccessTokenAsync(authUser);
            var (access, expires) = await _tokens.CreateAccessTokenAsync(authUser);

            await _context.SaveChangesAsync();
            return Ok(new AuthResponse(access, newRefresh, expires));
        }

        // Get current user info
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<object>> Me()
        {
            var userId = GetUserIdFromClaims();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { error = "No user id claim in token." });
            }

            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                return Unauthorized(new { error = "User not found." });
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


        // Logout (revoke refresh token)
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var userId = GetUserIdFromClaims();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { error = "No user id claim in token." });
            }

            var rt = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && t.UserId == userId, cancellationToken);

            if (rt is not null && !rt.Revoked)
            {
                rt.Revoked = true;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return NoContent();
        }
    }
}
