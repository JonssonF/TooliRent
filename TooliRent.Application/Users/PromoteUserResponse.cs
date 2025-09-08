using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users;

public sealed record PromoteUserResponse(
    string Id,
    string Email,
    string? FullName,
    IReadOnlyList<string> Roles
    );

