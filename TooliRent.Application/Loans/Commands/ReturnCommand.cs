using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Loans.Commands
{
    public sealed record ReturnCommand(int LoanId);
}
