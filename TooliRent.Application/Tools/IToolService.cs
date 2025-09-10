using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools;

public interface IToolService
{
    Task<IReadOnlyList<ToolListItemDto>> GetAsync(
        string? search,
        int? categoryId,
        ToolStatus? status,
        CancellationToken cancellationToken);
}
