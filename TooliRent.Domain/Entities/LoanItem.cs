using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Entities
{
    public class LoanItem
    {
        public int LoanId { get; set; }
        public Loan? Loan { get; set; }
        
        public int ToolId { get; set; }
        public Tool? Tool { get; set; }

        public string? ConditionOut { get; set; }
        public string? ConditionIn { get; set; }
    }
}
