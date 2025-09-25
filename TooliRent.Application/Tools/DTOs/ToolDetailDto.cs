using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs
{
    public sealed record ToolDetailDto
    (
        int Id,
        string Name,
        string? Description, 
        string? Manufacturer, 
        string? CategoryName, 
        string Status, 
        DateTime? LastMaintenanceDate, 
        DateTime? NextMaintenanceDate, 
        decimal PricePerDay,
        int TotalLoans
    );
}
