using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs
{
    public sealed record ToolDetailDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? Manufacturer { get; init; }
        public string? CategoryName { get; init; }
        public ToolStatus Status { get; init; }
        public DateTime? LastMaintenanceDate { get; init; }
        public DateTime? NextMaintenanceDate { get; init; }
        public decimal PricePerDay { get; init; }
        public int TotalLoans { get; init; }
    }
}
