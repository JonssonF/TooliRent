using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Bookings.DTOs
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<BookingItemDto> Items { get; set; } = new List<BookingItemDto>();
    }
}
