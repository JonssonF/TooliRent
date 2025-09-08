using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users;

// Promotes a user to the Admin role and optionally removes the Member role.
// Returns a response with the user's current roles after the operation.
public interface IAdminUserService
{
    Task<(bool ok, int status, string? error, PromoteUserResponse? data)> PromoteAsync(
        PromoteUserRequest request,
        CancellationToken cancellationToken = default);
}
