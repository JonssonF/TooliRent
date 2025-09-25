using Microsoft.EntityFrameworkCore;
using TooliRent.Application.Categories;
using TooliRent.Application.Categories.DTOs;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Categories
{

    // Injected 2 interfaces for segregation of read and write operations, i know its not optimal, but i didnt want to refactor everything now because of time.
    // In a real project, I would probably separate these into different classes.
    // Only reason i had to do this is because i had rows that was in domain, that i moved to Application to clean it up.
    public sealed class CategoryRepository : ICategoryRepository, ICategoryReadRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /*-------------------------WRITE ACTIONS (Domain)/*-------------------------*/
        public async Task AddAsync(ToolCategory category, CancellationToken cancellationToken = default)
        {
           await _context.Categories.AddAsync(category, cancellationToken);
        }
        public Task RemoveAsync(ToolCategory category, CancellationToken cancellationToken = default)
        {
            _context.Categories.Remove(category);
            return Task.CompletedTask;
        }
        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        {
           return _context.Categories
                .AsNoTracking()
                .Where(c => c.Name == name && (excludeId == null || c.Id != excludeId))
                .AnyAsync(cancellationToken);
        }

        /*-------------------------READ ACTIONS (APPLICATION)/*-------------------------*/
        public Task<ToolCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
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
