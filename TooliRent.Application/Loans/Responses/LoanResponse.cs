using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Loans.Responses
{
    public sealed record LoanResponse(
        int BookingId,
        int? LoanId,
        IReadOnlyList<int> ToolIds,
        string OldBookingStatus,
        string NewBookingStatus,
        DateTime TimeStamp
        );
    
    
}
