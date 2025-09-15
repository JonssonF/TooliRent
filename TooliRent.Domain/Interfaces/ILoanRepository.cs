using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;

namespace TooliRent.Domain.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> GetWithItemsAndBookingAsync(int loanId, CancellationToken cancellationToken);
        Task<Loan?> GetActiveByBookingIdAsync(int bookingId, CancellationToken cancellationToken);
        Task AddAsync(Loan loan, CancellationToken cancellationToken);
        Task UpdateAsync(Loan loan, CancellationToken cancellationToken);
        Task<IReadOnlyList<Loan>> GetAllActiveLoansAsync();
        Task<IReadOnlyList<Loan>> GetAllOverdueLoansAsync();
    }
}
