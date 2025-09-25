using TooliRent.Application.Categories.DTOs;

namespace TooliRent.Application.Categories
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<CategoryRow>> GetAllAsync(CancellationToken cancellationToken = default);
        //Task<CategoryRow?> GetByIdRowAsync(int id, CancellationToken cancellationToken = default);
    }
}
