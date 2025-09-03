using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users
{
    public record UserDto(string Id, string Email, string? FullName, List<string> Roles);
}
