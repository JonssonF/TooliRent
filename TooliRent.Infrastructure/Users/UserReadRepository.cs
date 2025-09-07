using Microsoft.EntityFrameworkCore;
using TooliRent.Domain.Users;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Users
{
    public sealed class UserReadRepository : IUserReadRepository
    {
        private readonly AppDbContext _context;

        public UserReadRepository(AppDbContext context)
        {
            _context = context;
        }

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
                .Select(u => new UserRow(u.Id, u.Email, u.FullName, u.UserName))
                .ToListAsync(cancellationToken);

            return (rows, total);
        }

        public async Task<Dictionary<string, List<string>>> GetRolesForUsersAsync(IEnumerable<string> userIds, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
