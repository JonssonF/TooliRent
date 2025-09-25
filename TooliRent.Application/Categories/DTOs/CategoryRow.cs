using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Domain.Categories
{
    public sealed record CategoryRow(
        int Id, 
        string Name, 
        string Description, 
        int ToolCount
    );
}
