using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }
        public string MemberId { get; set; } = string.Empty;
        public DateTime PickUpAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public DateTime DueAt { get; set; }
        public bool IsOverDue { get; set; }
        public ICollection<LoanItem> Items { get; set; } = new List<LoanItem>();
    }
}
