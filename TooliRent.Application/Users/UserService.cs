using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Common;
using TooliRent.Domain.Users;

namespace TooliRent.Application.Users
{
    public sealed class UserService : IUserService
    {
        private readonly IUserReadRepository _repo;

        public UserService(IUserReadRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<UserDto>> GetUsersPageAsync(int page, int pageSize, string? search, string? role, CancellationToken cancellationToken)
        {
            var (rows, total) = await _repo.GetUsersPageAsync(page, pageSize, search, role, cancellationToken);

            var roleMap = await _repo.GetRolesForUsersAsync(rows.Select(r => r.Id), cancellationToken);

            var items = rows.Select(r => new UserDto
            {
                Id = r.Id,
                Email = r.Email,
                FullName = r.FullName,
                Roles = roleMap.TryGetValue(r.Id, out var roles) ? roles : new List<string>()
            }).ToList();

            return new PagedResult<UserDto>(items, total, page, pageSize);
        }
    }
}
