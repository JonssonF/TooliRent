using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users;

public sealed class PromoteUserRequest
{
    public string Email { get; init; } = string.Empty;
    public bool RemoveMemberRole { get; init; } = false;
}
