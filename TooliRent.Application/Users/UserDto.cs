using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users
{
    public record UserDto {
        string Id { get; init; } = default!;
        string Email { get; init; } = default!;
        string? FullName { get; init; }
        public List<string> Roles { get; init; } = new List<string>();
    }
}
