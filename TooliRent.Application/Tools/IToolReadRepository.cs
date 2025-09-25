using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Interfaces;

public interface IToolReadRepository
{
    //Returns a list of tools based on optional filters: search term, category ID, and status.
    Task<IReadOnlyList<ToolListRow>> GetAsync(
        string? search,
        int? categoryId,
        ToolStatus? status,
        CancellationToken cancellationToken);


    // Counts how many Ids there is (used by create booking to validate tool ids)
    Task<int> CountExistingAsync(IEnumerable<int> toolIds, CancellationToken cancellationToken);

    //Returns a list of tools with their availability status within a specified date range, along with optional filters: search term and category ID.
    Task<IReadOnlyList<ToolAvailabilityRow>> GetAvailabilityAsync(
        DateTime startDate,
        DateTime endDate,
        string? search,
        int? categoryId,
        CancellationToken cancellationToken);

    // Retrieves detailed information about a specific tool by its ID.
    Task<ToolDetailRow?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    // Returns a dictionary mapping tool IDs to their last return dates. If a tool has never been returned, its value will be null.
    Task<Dictionary<int, DateTime?>> GetLastReturnDatesAsync(IEnumerable<int> toolIds, CancellationToken cancellationToken = default);

}
