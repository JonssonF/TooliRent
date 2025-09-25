using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Categories.DTOs
{
    public sealed record CategoryDto(
        int Id,
        string Name,
        string Description,
        int ToolCount
    );

}
