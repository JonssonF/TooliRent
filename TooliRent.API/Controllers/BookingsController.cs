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

        // Overkill helper method to get user id from various claim types depending on the auth provider.
        private string GetUserId()
        {
            return User.FindFirstValue("sub") // JWT standard
                ?? User.FindFirstValue("uid")  //Firebase, Auth0, Okta, Azure AD B2C, Cognito.
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier) // ASP.NET Identity, Azure AD.
                ?? User.Identity!.Name!; // Fallback to username if no other claim is found.
        }
        // Create a new booking, validating the request body, and returning the created booking with a 201 status code.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var (ok, error, data) = await _service.CreateAsync(request, GetUserId(), cancellationToken);
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
            var data = await _service.GetMineAsync(GetUserId(), cancellationToken);
            return Ok(data);
        }

        // Get a specific booking of the current user by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var data = await _service.GetMineByIdAsync(id, GetUserId(), cancellationToken);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            var (ok, error) = await _service.CancelAsync(id, GetUserId(), cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return NoContent();
        }
    }
}
