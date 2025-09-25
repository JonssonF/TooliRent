using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Categories;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Categories
{
    public sealed class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ToolCategory category, CancellationToken cancellationToken = default)
        {
           await _context.Categories.AddAsync(category, cancellationToken);
        }
        public Task RemoveAsync(ToolCategory category, CancellationToken cancellationToken = default)
        {
            _context.Categories.Remove(category);
            return Task.CompletedTask;
        }
        public Task<ToolCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
           return _context.Categories
                .AsNoTracking()
                .Where(c => c.Name == name && (excludeId == null || c.Id != excludeId))
                .AnyAsync(cancellationToken);
        }
        // Get all categories with the count of tools in each category
        public async Task<IReadOnlyList<CategoryRow>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                // First projecting to an anonymous type to include the tool count
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    ToolCount = _context.Tools.Count(t => t.CategoryId == c.Id)
                })
                .OrderBy(x => x.Name)
                // Projecting to CategoryRow DTO
                .Select(x => new CategoryRow(x.Id, x.Name, x.Description, x.ToolCount))
                .ToListAsync(cancellationToken);
        }
    }
}
