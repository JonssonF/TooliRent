using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Bookings.DTOs
{
    // DTO for listing bookings with essential information such as ID, dates, status, tool count, and cancellation eligibility.
    public class BookingListItemDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ToolCount { get; set; }
        public bool CanBeCancelled { get; set; }
    }
}
