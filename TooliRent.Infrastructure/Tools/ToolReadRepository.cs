using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;
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

        // Returns the count of tools that exist in the database from the provided list of tool IDs.
        public Task<int> CountExistingAsync(IEnumerable<int> toolIds, CancellationToken cancellationToken)
        {
            var ids = toolIds.Distinct().ToList();

            return _context.Tools
                .AsNoTracking()
                .Where(t => ids.Contains(t.Id))
                .CountAsync(cancellationToken);
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
                    (t.LastMaintenanceDate.HasValue && t.LastMaintenanceDate.Value.Date == DateTime.UtcNow.Date)
                    ? ToolStatus.Maintenance
                    : t.Status,
                    t.PricePerDay
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ToolAvailabilityRow>> GetAvailabilityAsync(DateTime startDate, DateTime endDate, string? search, int? categoryId, CancellationToken cancellationToken)
        {
            var blockingStatuses = new[] { BookingStatus.Pending, BookingStatus.Active };

            var tools = _context.Tools
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                tools = tools.Where(t =>
                   t.Name.ToLower().Contains(s) ||
                   (t.Description ?? string.Empty).ToLower().Contains(s) ||
                   t.Manufacturer.ToLower().Contains(s) ||
                   (t.SerialNumber ?? string.Empty).ToLower().Contains(s));
            }
            if (categoryId.HasValue)
            {
                tools = tools.Where(t => t.CategoryId == categoryId.Value);
            }

            return await tools
                .OrderBy(t => t.Name)
                .Select(t => new ToolAvailabilityRow(
                    t.Id,
                    t.Name,
                    t.Category != null ? t.Category.Name : null,
                    t.Status,
                    t.PricePerDay,
                    // Read up on positionarguments, couldnt define the boolean variable directly in the select. (AvailableInPeriod = ...)
                    !_context.Bookings.Any(b =>
                        blockingStatuses.Contains(b.Status) &&
                        b.StartDate < endDate && b.EndDate > startDate &&
                        b.Items.Any(i => i.ToolId == t.Id))
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<ToolDetailRow?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var query =
                from t in _context.Tools.AsNoTracking()
                where t.Id == id
                select new ToolDetailRow(
                    t.Id,
                    t.Name,
                    t.Description,
                    t.Manufacturer,
                    t.Category != null ? t.Category.Name : null,
                    t.Status,
                    t.LastMaintenanceDate,
                    t.NextMaintenanceDate,
                    t.PricePerDay,
                    _context.LoanItems.Count(li => li.ToolId == t.Id) // TotalLoans for this tool
                );

            return await query.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
