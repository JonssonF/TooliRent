using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Bookings
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking, CancellationToken cancellationToken)
        {
            await _context.Bookings.AddAsync(booking, cancellationToken);
        }
        public Task<Booking?> GetAggregateForUpdateAsync(int id, CancellationToken cancellationToken)
        {
            return _context.Bookings
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                        .ThenInclude(t => t.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public Task<Booking?> GetAggregateForUpdateForMemberAsync(int id, string memberId, CancellationToken cancellationToken)
        {
            return _context.Bookings
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                        .ThenInclude(t => t.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == id && b.MemberId == memberId, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<bool> AnyOverlappingAsync(IEnumerable<int> toolIds, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int? excludeBookingId = null)
        {
            var ids = toolIds.Distinct().ToList();

            var blockingStatuses = new[] { BookingStatus.Pending, BookingStatus.Active};

            return _context.Bookings
                    .AsNoTracking()
                    .Where(b => blockingStatuses.Contains(b.Status))
                    .Where(b => excludeBookingId == null || b.Id != excludeBookingId)
                    .Where(b => b.StartDate < endDate && startDate < b.EndDate)
                    .Where(b => b.Items.Any(i => ids.Contains(i.ToolId)))
                    .AnyAsync(cancellationToken);
        }

        public Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _context.Bookings
                .AsNoTracking()
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                        .ThenInclude(t => t.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public Task<Booking?> GetByIdForMemberAsync(int id, string memberId, CancellationToken cancellationToken)
        {
            return _context.Bookings
                .AsNoTracking()
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                        .ThenInclude(t => t.Category)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public Task<List<Booking>> GetForMemberAsync(string memberId, CancellationToken cancellationToken)
        {
            return _context.Bookings
                .AsNoTracking()
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                        .ThenInclude(t => t.Category)
                .AsSplitQuery()
                .Where(b => b.MemberId == memberId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Booking?> GetWithItemsAndToolsAsync(int bookingId, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Include(b => b.Items)
                    .ThenInclude(bi => bi.Tool)
                .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
        }

        // Helper method to set status of multiple tools.
        public async Task<int> SetToolsStatusAsync(IEnumerable<int> toolIds, ToolStatus status, CancellationToken cancellationToken = default)
        {
            var ids = toolIds.Distinct().ToList();
            var tools = await _context.Tools
                .Where(t => ids.Contains(t.Id))
                .ToListAsync(cancellationToken);

            foreach (var t in tools)
                t.Status = status;

            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
