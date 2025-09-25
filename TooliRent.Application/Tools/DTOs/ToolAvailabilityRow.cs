using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs
{
    public sealed record ToolAvailabilityRow(
        int Id,
        string Name,
        string? Category,
        ToolStatus Status,
        decimal PricePerDay,
        bool AvailableInPeriod);
}
