using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Common;

namespace TooliRent.Application.Users
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetUsersPageAsync(
            int page, int pageSize, string? search, string? role, CancellationToken cancellationToken);

        Task<IReadOnlyList<UserDto>> GetAllUsersSortedByRoleAsync(int max, CancellationToken cancellationToken);
    }
}
