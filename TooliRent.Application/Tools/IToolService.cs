using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools;

public interface IToolService
{
    Task<IReadOnlyList<ToolListItemDto>> GetAsync(
        string? search,
        int? categoryId,
        ToolStatus? status,
        CancellationToken cancellationToken);

    
    Task<IReadOnlyList<ToolAvailabilityRow>> GetAvailabilityAsync(
        DateTime startDate,
        DateTime endDate,
        string? search,
        int? categoryId,
        CancellationToken cancellationToken);

    // Retrieves detailed information about a specific tool by its ID.
    Task<ToolDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
