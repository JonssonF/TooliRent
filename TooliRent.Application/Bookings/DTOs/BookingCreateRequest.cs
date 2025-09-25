using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Bookings.DTOs
{
    // DTO for creating a new booking, including start and end dates and a list of tool IDs to be booked.
    public class BookingCreateRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<int> ToolIds { get; set; } = new List<int>();
    }
}
