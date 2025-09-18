using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users.DTOs
{
    public sealed record AdminUserResult(
        string Id,
        string Email,
        string FullName,
        IReadOnlyList<string> Roles,
        bool Suspended
        );
}
