using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Categories.DTOs
{
    // DTO for transferring category data, including ID, name, description, and tool count.
    public sealed record CategoryDto(
        int Id,
        string Name,
        string Description,
        int ToolCount
    );

}
