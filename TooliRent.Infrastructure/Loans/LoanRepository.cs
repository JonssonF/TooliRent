using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Persistence;
using TooliRent.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TooliRent.Infrastructure.Loans
{
    public sealed class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context) => _context = context;

        public async Task<Loan?> GetWithItemsAndBookingAsync(int loanId, CancellationToken cancellationToken)
        {
           return await _context.Loans
                .Include(l => l.Items).ThenInclude(li => li.Tool)
                .Include(l => l.Booking)
                .FirstOrDefaultAsync(l => l.Id == loanId, cancellationToken);
        }

        public async Task<Loan?> GetActiveByBookingIdAsync(int bookingId, CancellationToken cancellationToken)
        {
            return await _context.Loans
                .Include(l => l.Items).ThenInclude(li => li.Tool)
                .Include(l => l.Booking)
                .FirstOrDefaultAsync(l => l.BookingId == bookingId && l.ReturnedAt == null, cancellationToken);
        }
        
        public Task AddAsync(Loan loan, CancellationToken cancellationToken)
        {
            _context.Loans.Add(loan);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Loan loan, CancellationToken cancellationToken)
        {
            _context.Loans.Update(loan);
            return Task.CompletedTask;
        }

        // Get all active loans (not returned) Admin view
        public async Task<IReadOnlyList<Loan>> GetAllActiveLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.Items).ThenInclude(li => li.Tool)
                .Include(l => l.Booking)
                .Where(l => l.ReturnedAt == null)
                .ToListAsync();
        }
        // Get all overdue loans (not returned and past due date) Admin view
        public async Task<IReadOnlyList<Loan>> GetAllOverdueLoansAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Loans
                .Include(l => l.Items).ThenInclude(li => li.Tool)
                .Include(l => l.Booking)
                .Where(l => l.ReturnedAt == null && l.DueAt < now)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Loan>> GetActiveLoansByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.Loans
                .Include(l => l.Items).ThenInclude(li => li.Tool)
                .Include(l => l.Booking)
                .Where(l => l.ReturnedAt == null && l.MemberId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
