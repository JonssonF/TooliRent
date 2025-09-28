using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TooliRent.Application.Bookings;
using TooliRent.Application.Bookings.DTOs;

namespace TooliRent.API.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    [Authorize(Roles = "Member")]

    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;
        private readonly IValidator<BookingCreateRequest> _validator;

        public BookingsController(IBookingService service, IValidator<BookingCreateRequest> validator)
        {
            _service = service;
            _validator = validator;
        }

        /*------------Helper Method to find correct User throu claims----------------*/
        private string? GetUserIdFromClaims() =>
               User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub")
            ?? throw new InvalidOperationException("User ID claim not found.");
        /*---------------------------------------------------------------------------*/
        // Create a new booking, validating the request body, and returning the created booking.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                foreach (var er in validationResult.Errors)
                {
                    ModelState.AddModelError(er.PropertyName, er.ErrorMessage);
                }

                return ValidationProblem(ModelState);
            }

            var (ok, error, data) = await _service.CreateAsync(request, GetUserIdFromClaims(), cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            
            return CreatedAtAction(nameof(GetById), new { id = data!.Id }, data);
        }

        // Get all bookings of the current user
        [HttpGet("me")]
        public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
        {
            var data = await _service.GetMineAsync(GetUserIdFromClaims(), cancellationToken);
            return Ok(data);
        }

        // Get a specific booking of the current user by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _service.GetMineByIdAsync(id, GetUserIdFromClaims(), cancellationToken);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            var (ok, error) = await _service.CancelAsync(id, GetUserIdFromClaims(), cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return NoContent();
        }
    }
}
