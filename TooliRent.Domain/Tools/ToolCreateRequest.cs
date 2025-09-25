using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Tools
{
    public sealed record ToolCreateRequest(
        string Name,
        int CategoryId,
        decimal PricePerDay,
        string? Manufacturer = null,
        string? Description = null,
        string? SerialNumber = null
    );
}
