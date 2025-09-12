using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Bookings.DTOs
{
    public record BookingItemDto
    (
        int ToolId,
        string Name,
        string? Category,
        string? Status
    );

    
}
