using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools
{
    public sealed record ToolListItemDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? CategoryName { get; init; }
        public ToolStatus Status { get; init; }
        public decimal PricePerDay { get; init; }
    }
}
