using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Enums;

namespace TooliRent.Application.Tools.DTOs;

    public sealed record ToolDetailRow(
    int Id,
    string Name,
    string? Description,
    string? Manufacturer,
    string? CategoryName,
    ToolStatus Status,
    DateTime? LastMaintenanceDate,
    DateTime? NextMaintenanceDate,
    decimal PricePerDay,
    int TotalLoans
);

