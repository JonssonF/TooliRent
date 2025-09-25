using FluentValidation;
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
    [Route("api/loans")]
    [ApiController]
    [Authorize(Roles = "Admin, Member")]
    public sealed class LoansController : ControllerBase
    {
        private readonly ILoanService _loan;
        private readonly IValidator<PickupCommand> _pickupValidator;
        private readonly IValidator<ReturnCommand> _returnValidator;
        public LoansController(ILoanService loan, IValidator<PickupCommand> pickupValidator, IValidator<ReturnCommand> returnValidator)
        {
            _loan = loan;
            _pickupValidator = pickupValidator;
            _returnValidator = returnValidator;
        }

        /*------------Helper Method to find correct User throu claims----------------*/
        private string? GetUserIdFromClaims() =>
               User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID claim not found.");
        /*---------------------------------------------------------------------------*/

        // Pickup a tool (create a loan)
        [HttpPost("{bookingId:int}/pickup")]
        public async Task<IActionResult> Pickup([FromRoute] int bookingId, CancellationToken cancellationToken)
        {
            var validationResult = await _pickupValidator.ValidateAsync(new PickupCommand(bookingId), cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var response = await _loan.PickupAsync(new PickupCommand(bookingId),
                GetUserIdFromClaims()!, cancellationToken);
            return Ok(response);
        }

        // Return a tool (close a loan)
        [HttpPost("{loanId:int}/return")]
        public async Task<IActionResult> Return([FromRoute] int loanId, CancellationToken cancellationToken)
        {
            var validationResult = await _returnValidator.ValidateAsync(new ReturnCommand(loanId), cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
                var response = await _loan.ReturnAsync(new ReturnCommand(loanId),
                    GetUserIdFromClaims()!, cancellationToken);
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

        // Get active loans for the current user
        [HttpGet("my-active")]
        public async Task<IActionResult> GetMyActiveLoans(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var response = await _loan.GetActiveLoansByUserIdAsync(userId, cancellationToken);
            return Ok(response);
        }

        // Get all active loans (admin only)
        [HttpGet("all-active-loans")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActiveLoans(CancellationToken cancellationToken)
        {
            var response = await _loan.GetAllActiveLoansAsync(cancellationToken);
            return Ok(response);
        }

    }
}
