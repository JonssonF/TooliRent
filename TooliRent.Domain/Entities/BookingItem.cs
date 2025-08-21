using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Entities
{
    public class BookingItem
    {
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int ToolId { get; set; }
        public Tool? Tool { get; set; }
    }
}
