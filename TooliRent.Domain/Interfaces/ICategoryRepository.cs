using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;

namespace TooliRent.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(ToolCategory category, CancellationToken cancellationToken = default);
        Task RemoveAsync(ToolCategory category, CancellationToken cancellationToken = default);
        Task<ToolCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    }
}
