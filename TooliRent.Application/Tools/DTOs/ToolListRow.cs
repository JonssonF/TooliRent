using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs
{
    //Lightweight version of Tool entity for listing purposes. Purpose is to optimize data transfer and performance by not using "include".
    public sealed record ToolListRow
    (
        int Id,
        string Name,
        string? CategoryName,
        ToolStatus status,
        decimal PricePerDay
    );
    
}
