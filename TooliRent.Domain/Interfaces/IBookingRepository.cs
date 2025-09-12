using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Entities;

namespace TooliRent.Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task AddAsync(Booking booking, CancellationToken cancellationToken);
        Task<Booking?> GetAggregateForUpdateAsync(int id, CancellationToken cancellationToken);
        Task<Booking?> GetAggregateForUpdateForMemberAsync(int id, string memberId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> AnyOverlappingAsync(IEnumerable<int> toolIds, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int? excludeBookingId = null);
        Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Booking> GetByIdForMemberAsync(int id, string memberId, CancellationToken cancellationToken);
        Task<List<Booking>> GetForMemberAsync(string memberId, CancellationToken cancellationToken);
    }
}
