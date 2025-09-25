using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Interfaces
{
    public interface IToolAdminRepository
    {
        Task<Tool?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Tool tool, CancellationToken cancellationToken = default);
        Task DeleteAsync(Tool tool, CancellationToken cancellationToken = default);
        Task<List<Tool>> GetByStatusAsync(ToolStatus status, CancellationToken cancellationToken = default);
    }
}
