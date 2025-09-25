using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using TooliRent.Application.Tools;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Tools;

namespace TooliRent.API.Controllers
{
    [Route("api/tools")]
    [ApiController]
    [Authorize(Roles = "Admin, Member")]
    public sealed class ToolsController : ControllerBase
    {
        private readonly IToolService _service;

        public ToolsController(IToolService service)
        {
            _service = service;
        }
        // Get a list of tools with optional search, category, and status filters.
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<ToolListItemDto>>> Get(
            [FromQuery] string? search = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] Domain.Enums.ToolStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAsync(search, categoryId, status, cancellationToken);
            return Ok(result);
        }
        // Get tool availability within a specified date range, with optional search and category filters.
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
        // Get detailed information about a specific tool by its ID.
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ToolDetailDto>> GetById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto is null) return NotFound();
            return Ok(dto);
        }
    }
}
    

