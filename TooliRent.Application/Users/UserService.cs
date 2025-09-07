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

        // Get a page of users with optional search and role filtering
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

        // Get all users (up to 'max') sorted by their highest priority role and then by name/email
        public async Task<IReadOnlyList<UserDto>> GetAllUsersSortedByRoleAsync(int max, CancellationToken cancellationToken)
        {
            var (rows, _) = _repo.GetUsersPageAsync(1, max, null, null, cancellationToken).Result;

            var roleMap = await _repo.GetRolesForUsersAsync(rows.Select(r => r.Id), cancellationToken);

            var items = rows.Select(r => new UserDto
            {
                Id = r.Id,
                Email = r.Email ?? string.Empty,
                FullName = r.FullName,
                Roles = roleMap.TryGetValue(r.Id, out var roles) ? roles : new List<string>()
            });

            var sorted = items
                .Select(u => new
                {
                    User = u,
                    BestRolePriority = u.Roles.Select(RolePriority.GetPriority).DefaultIfEmpty(99).Min(),
                    NameForSort = u.FullName ?? u.Email
                })
                .OrderBy(x => x.BestRolePriority)
                .ThenBy(x => x.NameForSort)
                .Select(x => x.User)
                .ToList();

            return sorted;
        }
    }
}
