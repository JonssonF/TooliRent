using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools
{
    public class ToolService : IToolService
    {
        private readonly IToolReadRepository _toolRead;

        public ToolService(IToolReadRepository toolRead)
        {
            _toolRead = toolRead;
        }

        public async Task<IReadOnlyList<ToolListItemDto>> GetAsync(string? search, int? categoryId, ToolStatus? status, CancellationToken cancellationToken)
        {
            var rows = await _toolRead.GetAsync(search, categoryId, status, cancellationToken);

            return rows.Select(r => new ToolListItemDto
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.CategoryName,
                Status = r.status,
                PricePerDay = r.PricePerDay
            }).ToList();
        }
    }
}
