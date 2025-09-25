using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Tools
{
    public sealed class ToolAdminRepository : IToolAdminRepository
    {
        private readonly AppDbContext _context;

        public ToolAdminRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task<Tool?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
          return _context.Tools
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken); 
        }

        public async Task AddAsync(Tool tool, CancellationToken cancellationToken = default)
        {
           await _context.Tools.AddAsync(tool, cancellationToken);
        }
        public Task DeleteAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            _context.Tools.Remove(tool);
            return Task.CompletedTask;
        }

        public Task<List<Tool>> GetByStatusAsync(ToolStatus status, CancellationToken cancellationToken = default)
        {
            return _context.Tools
                .Where(t => t.Status == status)
                .ToListAsync(cancellationToken);
        }

        
    }
}
