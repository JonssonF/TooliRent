using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TooliRent.Domain.Identity;

namespace TooliRent.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        private readonly RoleManager<IdentityRole> _roles;
        private readonly UserManager<AppUser> _users;
        private readonly IConfiguration _cfg;
        private readonly ILogger<IdentitySeeder> _logger;

        public IdentitySeeder(
            RoleManager<IdentityRole> roles,
            UserManager<AppUser> users,
            IConfiguration cfg, 
            ILogger<IdentitySeeder> logger)
        {
            _cfg = cfg;
            _roles = roles;
            _users = users;
            _logger = logger;
        }

        /*
        Ensures default roles (Admin/Member) exist and a default admin user is created and assigned to both roles.
        Reads credentials from configuration: Seed:Admin:Email, Seed:Admin:Password, Seed:Admin:FullName.
        */

        public async Task SeedAsync()
        {
            // Ensure roles
            foreach (var role in new[] { Roles.Admin, Roles.Member })
            {
                if (!await _roles.RoleExistsAsync(role))
                {
                    var result = await _roles.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var msg = $"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}";
                        _logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }
                }
            }
            // Read admin user details from config
            var email = _cfg["Seed:Admin:Email"];
            var password = _cfg["Seed:Admin:Password"];
            var fullName = _cfg["Seed:Admin:FullName"] ?? "TooliRent Admin";
            var addMember = _cfg.GetValue<bool?>("Seed:Admin:AddMemberRole") ?? true;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Missing Seed:Admin Email/Password in configuration. ");
            }

            // Ensure admin user exists
            var admin = await _users.FindByEmailAsync(email);
            var createdNow = false;

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
                {
                    var msg = $"Failed to create admin user '{email}': {string.Join(", ", create.Errors.Select(e => e.Description))}";
                    _logger.LogError(msg);
                    throw new InvalidOperationException(msg);
                }

                createdNow = true;
                _logger.LogInformation("Default admin user created: {Email}", email);
            }
            // Ensure admin user is in both Admin and Member roles
            foreach (var role in new[] { Roles.Admin, Roles.Member })
            {
                if (!await _users.IsInRoleAsync(admin, role))
                {
                    var add = await _users.AddToRoleAsync(admin, role);
                    if (!add.Succeeded)
                    {
                        var msg = $"Failed to add admin user to '{role}': {string.Join(", ", add.Errors.Select(e => e.Description))}";
                        _logger.LogError(msg);
                        throw new InvalidOperationException(msg);
                    }
                    _logger.LogInformation("Added {Email} to role {Role}", email, role);
                }
            }

            if (createdNow && addMember && !await _users.IsInRoleAsync(admin, Roles.Member))
            {
                var add = await _users.AddToRoleAsync(admin, Roles.Member);
                if (!add.Succeeded)
                {
                    var msg = $"Failed to add admin user to role '{Roles.Member}': {string.Join(", ", add.Errors.Select(e => e.Description))}";
                    _logger.LogError(msg);
                    throw new InvalidOperationException(msg);
                }
                _logger.LogInformation("Added {Email} to role {Role}", email, Roles.Member);
            }
        }
    }
}   

