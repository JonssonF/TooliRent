using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Application.Categories;
using TooliRent.Application.Categories.DTOs;
using TooliRent.Domain.Entities;

namespace TooliRent.API.Controllers.AdminOnly
{
    [Route("api/admin/categories")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public AdminCategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var rows = await _service.GetAllAsync(cancellationToken);
            return Ok(rows);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request, CancellationToken cancellationToken)
        {
            var (ok, error, data) = await _service.CreateAsync(request, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return CreatedAtAction(nameof(GetAll), new { id = data!.Id }, data);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateRequest request, CancellationToken cancellationToken)
        {
            var (ok, error, data) = await _service.UpdateAsync(id, request, cancellationToken);
            if (!ok)
            {
                return BadRequest(new { error });
            }
            return Ok(data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
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
