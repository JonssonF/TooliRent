using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Tools;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Tools;

namespace TooliRent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public sealed class ToolsController : ControllerBase
    {
        private readonly IToolService _service;

        public ToolsController(IToolService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ToolListItemDto>>> Get(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] Domain.Enums.ToolStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAsync(search, categoryId, status, cancellationToken);
            return Ok(result);
        }

        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            CancellationToken cancellationToken = default)
        {
            var startUtc = startDate ?? DateTime.UtcNow;
            var endUtc = endDate ?? startUtc.AddDays(1);

            try
            {
                var rows = await _service.GetAvailabilityAsync(startUtc, endUtc, search, categoryId, cancellationToken);
                return Ok(rows);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
