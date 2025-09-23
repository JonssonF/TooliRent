using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;

namespace TooliRent.Domain.Interfaces
{
    public interface IToolRepository
    {
        Task AddAsync(Tool tool, CancellationToken cancellationToken = default);
        Task RemoveAsync(Tool tool, CancellationToken cancellationToken = default);
        Task<Tool?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
