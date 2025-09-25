using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs
{
    public sealed record ToolListItemDto
    (
        int Id,
        string Name,
        string? CategoryName,
        string Status,
        decimal PricePerDay
    );
}
