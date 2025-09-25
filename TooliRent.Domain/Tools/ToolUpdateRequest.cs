using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Tools
{
    public sealed record ToolUpdateRequest(
        
        string Name,
        int CategoryId,
        decimal PricePerDay,
        string? Manufacturer = null,
        string? Description = null,
        string? SerialNumber = null,
        ToolStatus? Status = null
    );
}
