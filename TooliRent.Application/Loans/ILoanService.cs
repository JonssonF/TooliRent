using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Loans.Commands;
using TooliRent.Application.Loans.Responses;

namespace TooliRent.Application.Loans
{
    public interface ILoanService
    {
            Task<LoanResponse> PickupAsync(PickupCommand cmd, string userId, CancellationToken cancellationToken = default);
            Task<LoanResponse> ReturnAsync(ReturnCommand cmd, string userId, CancellationToken cancellationToken = default);
            Task<int> MarkOverduesAsync(CancellationToken cancellationToken = default);
            Task<IReadOnlyList<ActiveLoanResponse>> GetActiveLoansByUserIdAsync(string userId, CancellationToken cancellationToken = default);
            Task<IReadOnlyList<Responses.ActiveLoanResponse>> GetAllActiveLoansAsync(CancellationToken cancellationToken = default);

    }
}
