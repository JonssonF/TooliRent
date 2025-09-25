using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Application.Categories.DTOs
{
    // DTO for creating a new category with a name and description.
    public sealed record CategoryCreateRequest(
        string Name,
        string Description
    );

}
