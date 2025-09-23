using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Interfaces;
using TooliRent.Domain.Tools;
using TooliRent.Infrastructure.Persistence;

namespace TooliRent.Infrastructure.Tools
{
    public sealed class ToolRepository : IToolRepository
    {
        private readonly AppDbContext _context;

        public ToolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            await _context.Tools.AddAsync(tool, cancellationToken);
        }

        public Task<Tool?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return _context.Tools
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public Task RemoveAsync(Tool tool, CancellationToken cancellationToken = default)
        {
            _context.Tools.Remove(tool);
            return Task.CompletedTask;
        }
    }
}
