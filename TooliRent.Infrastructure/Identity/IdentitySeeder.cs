using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Identity;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roles;
        private readonly UserManager<AppUser> _users;
        private readonly IConfiguration _cfg;
        private readonly ILogger<IdentitySeeder> _logger;

        public IdentitySeeder(RoleManager<IdentityRole> roles, UserManager<AppUser> users, IConfiguration cfg, ILogger<IdentitySeeder> logger)
        {
            _cfg = cfg;
            _roles = roles;
            _users = users;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            foreach (var role in new[] { "Admin", "Member" })
            {
                if (!await _roles.RoleExistsAsync(role))
                {
                    var result = await _roles.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new Exception($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var email = _cfg["Seed:Admin:Email"];
            var password = _cfg["Seed:Admin:Password"];
            var fullName = _cfg["Seed:Admin:FullName"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Missing Seed:Admin Email/Password in configuration. ");
            }

            var admin = await _users.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName
                };

                var create = await _users.CreateAsync(admin, password);
                if (!create.Succeeded)
                    throw new Exception($"Failed to create admin user: {string.Join(", ", create.Errors.Select(e => e.Description))}");
            }

            if (!await _users.IsInRoleAsync(admin, "Admin"))
            {
                var add = await _users.AddToRoleAsync(admin, "Admin");
                if (!add.Succeeded)
                    throw new Exception($"Failed to add admin user to {Roles.Admin}: {string.Join(", ", add.Errors.Select(e => e.Description))}");
            }
        }
    }
    }

