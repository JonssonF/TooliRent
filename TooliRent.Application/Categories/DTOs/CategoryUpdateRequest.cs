using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Categories.DTOs
{
    public sealed record CategoryUpdateRequest(
        string Name,
        string Description
    );


}
