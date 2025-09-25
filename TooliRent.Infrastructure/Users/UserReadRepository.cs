using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TooliRent.Domain.Interfaces;
using TooliRent.Application.Users.DTOs;
using TooliRent.Infrastructure.Persistence;
using TooliRent.Application.Tools;

namespace TooliRent.Infrastructure.Users
{
    public sealed class UserReadRepository : IUserReadRepository
    {
        private readonly AppDbContext _context;

        public UserReadRepository(AppDbContext context)
        {
            _context = context;
        }
        // Get a paged list of users, with optional search and role filtering
        public async Task<(IReadOnlyList<UserRow> Rows, int Total)> GetUsersPageAsync(int page, int pageSize, string? search, string? role, CancellationToken cancellationToken)
        {
            if (page < 1)
            {
                page = 1;
            }
            if (pageSize < 1 || pageSize > 200)
            {
                pageSize = 50;
            }

            var query = _context.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(u =>
                    u.UserName!.ToLower().Contains(search) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(search))
                );
            }

            if (!string.IsNullOrWhiteSpace(role))
            {
                var r = role.Trim();
                query = from u in query
                        join ur in _context.UserRoles.AsNoTracking() on u.Id equals ur.UserId
                        join rl in _context.Roles.AsNoTracking() on ur.RoleId equals rl.Id
                        where rl.Name == r
                        select u;
            }

            var total = await query.CountAsync(cancellationToken);

            var rows = await query
                .OrderBy(u => u.UserName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserRow(
                    u.Id,
                    u.Email,
                    u.FullName,
                    u.UserName,
                    u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken);

            return (rows, total);
        }
        // Get roles for a list of user IDs
        public async Task<Dictionary<string, List<string>>> GetRolesForUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            var ids =  userIds.Distinct().ToList();
            if(ids.Count == 0)
            {
                return new Dictionary<string, List<string>>();
            }

            var roles =  await (from ur in _context.UserRoles.AsNoTracking()
                                join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                where ids.Contains(ur.UserId)
                                select new { ur.UserId, r.Name })
                    .ToListAsync(cancellationToken);
            
            return roles
                .GroupBy(x => x.UserId)
                .ToDictionary(
                g => g.Key, 
                g => g.Select(x => x.Name!)
                            .Where(n => n != null)
                            .OrderBy(n => n)
                            .ToList());
        }

    }
}
