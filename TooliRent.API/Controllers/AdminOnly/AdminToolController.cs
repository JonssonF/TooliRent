using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Tools;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Tools;

namespace TooliRent.API.Controllers.AdminOnly
{
    [Route("api/admin/tools")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class AdminToolController : ControllerBase
    {
        private readonly IToolAdminService _service;

        public AdminToolController(IToolAdminService service)
        {
            _service = service;
        }
        // Create a new tool
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ToolCreateRequest request, CancellationToken cancellationToken)
        {
            var (ok, error, id) = await _service.CreateAsync(request, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return CreatedAtAction(nameof(Create), new { id }, new { id });
        }
        // Update an existing tool
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ToolUpdateRequest request, CancellationToken cancellationToken)
        {
            var (ok, error) = await _service.UpdateAsync(id, request, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return NoContent();
        }
        // Delete a tool
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var (ok, error) = await _service.DeleteAsync(id, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return NoContent();
        }
    }
}
