using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public string MemberId { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
    }
}
