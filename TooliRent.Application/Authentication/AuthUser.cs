using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Authentication
{
    // Represents an authenticated user with their details and roles.
    public sealed record AuthUser(
        string Id,
        string? Email,
        string? UserName,
        IReadOnlyList<string> Roles
        );
}
