using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Tools;

namespace TooliRent.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ToolsController : ControllerBase
    {
        private readonly IToolService _service;

        public ToolsController(IToolService service)
        {
            _service = service;
        }

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
    }
}
