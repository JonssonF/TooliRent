using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Tools
{
    public sealed record ToolAvailabilityRow(
        int Id,
        string Name,
        string? Category,
        ToolStatus Status,
        decimal PricePerDay,
        bool AvailableInPeriod);
}
