using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Entities;

namespace TooliRent.Domain.Tools;

public interface IToolReadRepository
{
    //Returns a list of tools based on optional filters: search term, category ID, and status.

    Task<IReadOnlyList<Tool>> GetAsync(
        string? search,
        int? categoryId,
        ToolStatus? status,
        CancellationToken cancellationToken);
}
