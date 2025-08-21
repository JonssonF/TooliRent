using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Domain.Entities
{
    public class Tool
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Manufacturer { get; set; } = string.Empty;
        public string? SerialNumber { get; set; }
        public int CategoryId { get; set; }
        public ToolCategory? Category { get; set; }
        public ToolStatus Status { get; set; } = ToolStatus.Available;
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public decimal PricePerDay { get; set; } = 0.0m;
        public decimal PricePerHour { get; set; } = 0.0m;
    }
}
