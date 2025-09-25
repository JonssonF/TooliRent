using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools
{
    public interface IToolAdminService
    {
        Task<(bool ok, string? error, int id)> CreateAsync(ToolCreateRequest request, CancellationToken cancellationToken = default);
        Task<(bool ok, string? error)> UpdateAsync(int toolId, ToolUpdateRequest request, CancellationToken cancellationToken = default);
        Task<(bool ok, string? error)> DeleteAsync(int toolId, CancellationToken cancellationToken = default);
        Task<int> CompleteMaintenanceAsync(CancellationToken cancellationToken = default);
    }
}
