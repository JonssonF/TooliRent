using AutoMapper;
using TooliRent.Application.Tools.DTOs;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Interfaces;

namespace TooliRent.Application.Tools
{
    public class ToolService : IToolService
    {
        private readonly IToolReadRepository _toolRead;
        private readonly IMapper _mapper;

        public ToolService(IToolReadRepository toolRead, IMapper mapper)
        {
            _toolRead = toolRead;
            _mapper = mapper;
        }
        // Retrieves a list of tools based on optional filters such as search term, category ID, and status.
        public async Task<IReadOnlyList<ToolListItemDto>> GetAsync(string? search, int? categoryId, ToolStatus? status, CancellationToken cancellationToken)
        {
            var rows = await _toolRead.GetAsync(search, categoryId, status, cancellationToken);
            return _mapper.Map<IReadOnlyList<ToolListItemDto>>(rows);
        }

        // Ensures the DateTime is in UTC. If the Kind is Unspecified, it assumes UTC.
        private static DateTime EnsureUtc(DateTime dt) =>
            dt.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) :
                dt.ToUniversalTime();
      
        // Retrieves the availability of tools within a specified date range, applying optional filters such as search term and category ID.
        public async Task<IReadOnlyList<ToolAvailabilityRow>> GetAvailabilityAsync(
            DateTime startDate, 
            DateTime endDate, 
            string? search, 
            int? categoryId, 
            CancellationToken cancellationToken)
        {
            startDate = EnsureUtc(startDate);
            endDate = EnsureUtc(endDate);
            if (endDate <= startDate)
            {
                throw new ArgumentException("End date cannot be earlier than start date.");
            }
            return await _toolRead.GetAvailabilityAsync(startDate, endDate, search, categoryId, cancellationToken);
        }

        // Retrieves detailed information about a specific tool by its ID.
        public async Task<ToolDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var row = await _toolRead.GetDetailByIdAsync(id, cancellationToken);
            return row is null ? null : _mapper.Map<ToolDetailDto>(row);
        }
    }
}
