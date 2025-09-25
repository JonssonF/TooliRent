using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Users;

namespace TooliRent.Domain.Interfaces
{
    public interface IUserReadRepository
    {
        Task<(IReadOnlyList<UserRow> Rows, int Total)> GetUsersPageAsync(
            int page, int pageSize, string? search, string? role, CancellationToken cancellationToken);

        Task<Dictionary<string, List<string>>> GetRolesForUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken);
    }
}
