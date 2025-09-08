using Microsoft.AspNetCore.Identity;
using TooliRent.Infrastructure.Identity;
using TooliRent.Application.Users;
using FluentValidation;

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

        public async Task<(bool ok, int status, string? error, PromoteUserResponse? data)> PromoteAsync(PromoteUserRequest request, CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(request.Email))
                return (false, 400, "Email is required.", null);

            var user = await _users.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return (false, 404, "User not found.", null);
            }

            if (!await _roles.RoleExistsAsync("Admin"))
            {
                return (false, 400, "Role 'Admin' does not exist.", null);
            }

            if (!await _users.IsInRoleAsync(user, "Admin"))
            {
                var add = await _users.AddToRoleAsync(user, "Admin");
                if(!add.Succeeded)
                {
                    var errors = string.Join(", ", add.Errors.Select(e => e.Description));
                    return (false, 400, errors, null);
                }
            }
            
            if(request.RemoveMemberRole && await _users.IsInRoleAsync(user, "Member"))
            {
                var rem = await _users.RemoveFromRoleAsync(user, "Member");
                if (!rem.Succeeded)
                {
                    var errors = string.Join(", ", rem.Errors.Select(e => e.Description));
                    return (false, 400, errors, null);
                }
            }
            
            var roles = await _users.GetRolesAsync(user);

            var response = new PromoteUserResponse(
            
                user.Id,
                user.Email ?? string.Empty,
                user.FullName,
                roles.OrderBy(r => r).ToArray()
            );

            return (true, 200, null, response);
        }



    }
}
