using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users.DTOs
{
    public record UserDto
    {
        public string Id { get; init; } = default!;
        public string Email { get; init; } = default!;
        public string? FullName { get; init; }

        // Roles is not populated by AutoMapper, it is fetched through a query in UsersController
        public List<string> Roles { get; init; } = new List<string>();
    }
}
