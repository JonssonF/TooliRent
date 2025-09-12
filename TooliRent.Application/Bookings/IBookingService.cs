using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;

namespace TooliRent.Application.Bookings
{
    public interface IBookingService
    {
        Task<(bool Ok, string? Error, BookingDetailsDto? Data)> CreateAsync(
            BookingCreateRequest request,
            string memberId,
            CancellationToken cancellationToken);

        Task<List<BookingListItemDto>> GetMineAsync(
            string memberId,
            CancellationToken cancellationToken);

        Task<BookingDetailsDto?> GetMineByIdAsync(
            int id,
            string memberId,
            CancellationToken cancellationToken);

        Task<(bool Ok, string? Error)> CancelAsync(
            int id,
            string memberId,
            CancellationToken cancellationToken);
    }
}
