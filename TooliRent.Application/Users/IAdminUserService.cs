using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Users.DTOs;

namespace TooliRent.Application.Users;

public interface IAdminUserService
{
    Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> PromoteAsync(
            PromoteUserRequest request,
            CancellationToken cancellationToken = default);

    Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> DemoteAsync(
        DemoteUserRequest request,
        CancellationToken cancellationToken = default);

    Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> DeactivateAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<(bool ok, HttpStatusCode status, string? error, AdminUserResult? data)> ActivateAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
