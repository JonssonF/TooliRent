using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Categories.DTOs;

namespace TooliRent.Application.Categories
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(bool ok, string? error, CategoryDto? data)> CreateAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);
        Task<(bool ok, string? error, CategoryDto? data)> UpdateAsync(int id, CategoryUpdateRequest request, CancellationToken cancellationToken = default);
        Task<(bool ok, string? error)> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
