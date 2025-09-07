using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Users
{
    public interface IAdminUserService
    {
        Task<(bool ok, int status, string? error, PromoteUserResponse? data)> PromoteAsync(
            PromoteUserRequest request,
            CancellationToken cancellationToken = default);
    }
}
