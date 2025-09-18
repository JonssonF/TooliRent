using Microsoft.AspNetCore.Identity;
using TooliRent.Infrastructure.Identity;
using TooliRent.Application.Users;
using FluentValidation;
using TooliRent.Application.Users.DTOs;
using System.Net;
using TooliRent.Domain.Identity;

namespace TooliRent.Infrastructure.Users
{
    public sealed class AdminUserService : IAdminUserService
    {
        private readonly UserManager<AppUser> _users;
        private readonly RoleManager<IdentityRole> _roles;

        public AdminUserService(UserManager<AppUser> users, RoleManager<IdentityRole> roles) 
        {
            _users = users;
            _roles = roles;
        }
        // Promote a user to Admin role, optionally removing Member role
        public async Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> PromoteAsync(
            PromoteUserRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _users.FindByEmailAsync(request.Email);
            if (user is null) 
            {
                return (false, HttpStatusCode.NotFound, "User not found.", null);
            }

            if (!await _roles.RoleExistsAsync(Roles.Admin))
            {
                return (false, HttpStatusCode.BadRequest, $"Role '{Roles.Admin}' does not exist.", null);
            }

            if (!await _users.IsInRoleAsync(user, Roles.Admin))
            {
                var add = await _users.AddToRoleAsync(user, Roles.Admin);
                if (!add.Succeeded)
                    return (false, HttpStatusCode.BadRequest, string.Join("; ", add.Errors.Select(e => e.Description)), null);
            }

            if (request.RemoveMemberRole && await _users.IsInRoleAsync(user, Roles.Member))
            {
                var rem = await _users.RemoveFromRoleAsync(user, Roles.Member);
                if (!rem.Succeeded)
                    return (false, HttpStatusCode.BadRequest, string.Join("; ", rem.Errors.Select(e => e.Description)), null);
            }

            var roles = await _users.GetRolesAsync(user);
            var suspended = await _users.IsLockedOutAsync(user);
            var dto = new AdminUserResult(
                user.Id,
                user.Email!,
                user.FullName,
                roles.ToList(),
                suspended
                );

            return (true, HttpStatusCode.OK, null, dto);
        }
        // Promote a user to Admin role, optionally removing Member role
        public async Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> DemoteAsync(
            DemoteUserRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _users.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return (false, HttpStatusCode.NotFound, "User not found.", null);
            }

            if (!await _roles.RoleExistsAsync(Roles.Admin))
            {
                return (false, HttpStatusCode.BadRequest, $"Role '{Roles.Admin}' does not exist.", null);
            }

            if (await _users.IsInRoleAsync(user, Roles.Admin))
            {
                var rem = await _users.RemoveFromRoleAsync(user, Roles.Admin);
                if (!rem.Succeeded) 
                { 
                    return (false, HttpStatusCode.BadRequest, string.Join("; ", rem.Errors.Select(e => e.Description)), null);
                }
            }

            if (request.AddMemberRole && !await _users.IsInRoleAsync(user, Roles.Member))
            {
                if (!await _roles.RoleExistsAsync(Roles.Member))
                {
                    return (false, HttpStatusCode.BadRequest, $"Role '{Roles.Member}' does not exist.", null);
                }

                var add = await _users.AddToRoleAsync(user, Roles.Member);
                if (!add.Succeeded)
                {
                    return (false, HttpStatusCode.BadRequest, string.Join("; ", add.Errors.Select(e => e.Description)), null);
                }
            }

            var roles = await _users.GetRolesAsync(user);
            var suspended = await _users.IsLockedOutAsync(user);
            var dto = new AdminUserResult(
                user.Id,
                user.Email!,
                user.FullName,
                roles.ToList(),
                suspended
                );

            return (true, HttpStatusCode.OK, null, dto);
        }

        // Deactivate a user by setting lockout
        public async Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> DeactivateAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                return (false, HttpStatusCode.NotFound, "User not found.", null);
            }

            await _users.SetLockoutEnabledAsync(user, true);
            await _users.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            var roles = await _users.GetRolesAsync(user);
            var suspended = await _users.IsLockedOutAsync(user);

            var dto = new AdminUserResult(
                user.Id,
                user.Email!,
                user.FullName,
                roles.ToList(),
                suspended
                );

            return (true, HttpStatusCode.NoContent, null, dto);
        }

        // Activate a user by removing lockout
        public async Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> ActivateAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                return (false, HttpStatusCode.NotFound, "User not found.", null);
            }

            await _users.SetLockoutEndDateAsync(user, null);

            var roles = await _users.GetRolesAsync(user);
            var suspended = await _users.IsLockedOutAsync(user);

            var dto = new AdminUserResult(
                user.Id,
                user.Email!,
                user.FullName,
                roles.ToList(),
                suspended
                );

            return (true, HttpStatusCode.NoContent, null, dto);
        }
    }
}
