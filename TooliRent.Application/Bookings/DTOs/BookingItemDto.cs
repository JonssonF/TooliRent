using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Bookings.DTOs
{
    // DTO representing an individual item in a booking, including tool details.
    public sealed record BookingItemDto
    (
        int ToolId,
        string ToolName,
        string? CategoryName,
        string? Status
    );

    
}
