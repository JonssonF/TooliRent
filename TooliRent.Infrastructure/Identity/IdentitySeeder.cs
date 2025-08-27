using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Identity;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Identity
{
    public class IdentitySeeder
    {
        private readonly IConfiguration _cfg;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly ILogger<IdentitySeeder> _logger;
        private readonly UserManager<AppUser> _users;

        public IdentitySeeder(RoleManager<IdentityRole> roles, UserManager<AppUser> users, IConfiguration cfg, ILogger<IdentitySeeder> logger)
        {
            _cfg = cfg;
            _roles = roles;
            _users = users;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            foreach (var role in new[] { Roles.Admin, Roles.Member })
            {
                if (!await _roles.RoleExistsAsync(role))
                {
                    var result = await _roles.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        throw new Exception($"Kunde inte skapa roll '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            
            var email = _cfg["Seed:Admin:Email"];
            var password = _cfg["Seed:Admin:Password"];
            var firstName = _cfg["Seed:Admin:FirstName"];
            var lastName = _cfg["Seed:Admin:LastName"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Seed:Admin missing Email/Password in appsettings. Skipping admin-seed.");
                return;
            }

            var admin = await _users.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new AppUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName
                };

                var create = await _users.CreateAsync(admin, password);
                if (!create.Succeeded)
                    throw new Exception($"Could not create admin: {string.Join(", ", create.Errors.Select(e => e.Description))}");
            }

            
            if (!await _users.IsInRoleAsync(admin, Roles.Admin))
            {
                var add = await _users.AddToRoleAsync(admin, Roles.Admin);
                if (!add.Succeeded)
                    throw new Exception($"Could not add admin in {Roles.Admin}: {string.Join(", ", add.Errors.Select(e => e.Description))}");
            }
        }
    }
    }

