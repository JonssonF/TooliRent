using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooliRent.Application.Loans;
using TooliRent.Application.Loans.Commands;
using TooliRent.Domain.Identity;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Member")]
    public sealed class LoansController : ControllerBase
    {
        private readonly ILoanService _loan;

        public LoansController(ILoanService loan)
        {
            _loan = loan;
        }

        /*------------Helper Method to find correct User throu claims----------------*/
        private string? GetUserIdFromClaims() =>
               User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID claim not found.");
        /*---------------------------------------------------------------------------*/

        private bool IsAdmin() => User.IsInRole(Roles.Admin);

        // Pickup a tool (create a loan)
        [HttpPost("{bookingId:int}/pickup")]
        public async Task<IActionResult> Pickup([FromRoute] int bookingId, CancellationToken cancellationToken)
        {
            var response = await _loan.PickupAsync(new PickupCommand(bookingId),
                GetUserIdFromClaims()!, IsAdmin(), cancellationToken);
            return Ok(response);
        }

        // Return a tool (close a loan)
        [HttpPost("{loanId:int}/return")]
        public async Task<IActionResult> Return([FromRoute] int loanId, CancellationToken cancellationToken)
        {
            var response = await _loan.ReturnAsync(new ReturnCommand(loanId),
                GetUserIdFromClaims()!, IsAdmin(), cancellationToken);
            return Ok(response);
        }

        // Mark overdue loans (admin only)
        [HttpPost("mark-overdues")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkOverdues(CancellationToken cancellationToken)
        {
            var updated = await _loan.MarkOverduesAsync(cancellationToken);
            return Ok(new { updated });
        }

    }
}
