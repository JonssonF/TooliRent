namespace TooliRent.Application.Tools.DTOs
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
