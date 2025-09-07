using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users
{
    public record UserDto {
       public string Id { get; init; } = default!;
       public string Email { get; init; } = default!;
       public string? FullName { get; init; }
       public List<string> Roles { get; init; } = new List<string>();
    }
}
