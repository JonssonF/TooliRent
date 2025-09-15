using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Loans.Commands;
using TooliRent.Application.Loans.Responses;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;

namespace TooliRent.Application.Loans
{
    public sealed class LoanService : ILoanService
    {
        private readonly IBookingRepository _bookings;
        private readonly ILoanRepository _loans;
        private readonly IUnitOfWork _uow;

        public LoanService(IBookingRepository bookings, ILoanRepository loans, IUnitOfWork uow)
        {
            _bookings = bookings;
            _loans = loans;
            _uow = uow;
        }

        public async Task<LoanResponse> PickupAsync(PickupCommand cmd, string userId, bool asAdmin = false, CancellationToken cancellationToken = default)
        {
            var booking = await _bookings.GetWithItemsAndToolsAsync(cmd.BookingId, cancellationToken);
            if (booking is null)
                throw new KeyNotFoundException("Booking not found.");

            if (!asAdmin && booking.MemberId != userId)
                throw new UnauthorizedAccessException("You are not allowed to pickup this booking.");

            if (booking.Status != BookingStatus.Pending)
                throw new InvalidOperationException($"Booking must be Pending (was {booking.Status}).");

            var now = DateTime.UtcNow;
            if (now < booking.StartDate.AddHours(-1))
                throw new InvalidOperationException("Too early to pick up.");
            if (now > booking.EndDate)
                throw new InvalidOperationException("Booking window has passed.");

            var existing = await _loans.GetActiveByBookingIdAsync(booking.Id, cancellationToken);
            if (existing is not null)
                throw new InvalidOperationException("Loan already exists for this booking.");

            
            foreach (var bi in booking.Items)
            {
                var tool = bi.Tool!;
                if (tool.Status != ToolStatus.Available)
                {
                    throw new InvalidOperationException($"Tool {tool.Id} is {tool.Status}.");
                }
                bi.Tool!.Status = ToolStatus.Rented;
            }

            var loan = new Loan
            {
                BookingId = booking.Id,
                MemberId = booking.MemberId,
                PickUpAt = now,
                DueAt = booking.EndDate,
                Items = booking.Items.Select(i => new LoanItem {ToolId = i.ToolId}).ToList()
            };
            
            var oldStatus = booking.Status.ToString();
            booking.Status = BookingStatus.Active;

            await _loans.AddAsync(loan, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new LoanResponse(
                BookingId: booking.Id,
                LoanId: loan.Id,
                ToolIds: loan.Items.Select(x => x.ToolId).ToList(),
                OldBookingStatus: oldStatus,
                NewBookingStatus: booking.Status.ToString(),
                TimeStamp: now
            );
        }
        
        public async Task<LoanResponse> ReturnAsync(ReturnCommand cmd, string userId, bool asAdmin = false, CancellationToken cancellationToken = default)
        {
            // Load loan with items and booking
            var loan = await _loans.GetWithItemsAndBookingAsync(cmd.LoanId, cancellationToken);
            if (loan is null)
            {
                throw new KeyNotFoundException("Loan not found.");
            }
            if (!asAdmin && loan.MemberId != userId)
            {
                throw new UnauthorizedAccessException("You are not allowed to return this loan.");
            }
            if (loan.ReturnedAt is not null)
            {
                throw new InvalidOperationException("Loan already returned.");
            }

            var booking = loan.Booking!;
            if (booking.Status != BookingStatus.Active)
            {
                throw new InvalidOperationException($"Booking must be Active (was {booking.Status}).");
            }

            var now = DateTime.UtcNow;
                
            // Mark as overdue if returning after due date
            if(now > loan.DueAt && !loan.IsOverDue)
            {
                loan.IsOverDue = true;
                loan.DueAt = now;
            }

            loan.ReturnedAt = now;

            // Update tool statuses
            foreach (var li in loan.Items)
            {
                if (li.Tool is not null)
                {
                    li.Tool!.Status = ToolStatus.Available;
                }
            }

            var oldStatus = booking.Status.ToString();
            booking.Status = BookingStatus.Completed;
            
            await _uow.SaveChangesAsync(cancellationToken);
            
            return new LoanResponse(
                BookingId: booking.Id,
                LoanId: loan.Id,
                ToolIds: loan.Items.Select(x => x.ToolId).ToList(),
                OldBookingStatus: oldStatus,
                NewBookingStatus: booking.Status.ToString(),
                TimeStamp: now
            );
        }

        public async Task<int> MarkOverduesAsync(CancellationToken cancellationToken = default)
        {
            var overdueLoans = await _loans.GetAllOverdueLoansAsync();

            var now = DateTime.UtcNow;
            var newlyMarked = 0;

            foreach (var loan in overdueLoans)
            {
                if(!loan.IsOverDue)
                {
                    loan.IsOverDue = true;
                    loan.DueAt = now;
                    newlyMarked++;
                }
            }

            if(newlyMarked > 0)
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }

                return newlyMarked;
        }

        public async Task<IReadOnlyList<ActiveLoanResponse>> GetActiveLoansByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var loans = await _loans.GetActiveLoansByUserIdAsync(userId, cancellationToken);

            return loans.Select(l => new ActiveLoanResponse(
                l.Id,
                l.BookingId,
                l.PickUpAt,
                l.DueAt,
                l.IsOverDue,
                l.Items.Select(i => i.ToolId).ToList()
            )).ToList();
        }

        public async Task<IReadOnlyList<ActiveLoanResponse>> GetAllActiveLoansAsync(CancellationToken cancellationToken = default)
        {
            var loans = await _loans.GetAllActiveLoansAsync();
            return loans.Select(l => new ActiveLoanResponse(
                l.Id,
                l.BookingId,
                l.PickUpAt,
                l.DueAt,
                l.IsOverDue,
                l.Items.Select(i => i.ToolId).ToList()
            )).ToList();
        }
    }
}
