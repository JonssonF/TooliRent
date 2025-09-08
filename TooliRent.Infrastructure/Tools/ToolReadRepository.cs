using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Tools;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Tools
{
    public sealed class ToolReadRepository : IToolReadRepository
    {
        private readonly AppDbContext _context;

        public ToolReadRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<ToolListRow>> GetAsync(string? search, int? categoryId, ToolStatus? status, CancellationToken cancellationToken)
        {
            var query = _context.Tools
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(t =>
                t.Name.Contains(s) ||
                (t.Description ?? "").Contains(s) ||
                t.Manufacturer.Contains(s) ||
                (t.SerialNumber ?? "").Contains(s));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            return await query
                .OrderBy(t => t.Name)
                .Select(t => new ToolListRow(
                
                    t.Id,
                    t.Name,
                    t.Category != null ? t.Category.Name : null,
                    t.Status,
                    t.PricePerDay
                ))
                .ToListAsync(cancellationToken);
        }
    }
}
