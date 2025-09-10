using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Bookings.DTOs;

namespace TooliRent.Application.Bookings.Validation
{
    public class BookingCreateRequestValidator : AbstractValidator<BookingCreateRequest>
    {
        public BookingCreateRequestValidator()
        {
            RuleFor(x => x.ToolIds)
                .NotEmpty().WithMessage("At least one tool must be selected for booking.")
                .Must(t => t.Distinct().Count() == t.Count)
                .WithMessage("Duplicate tool IDs are not allowed.");

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Start date must be before end date.");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => (endDate - request.StartDate).TotalHours >= 1)
                .WithMessage("We dont do ½ hour renting!");

            RuleFor(x => x)
                .Must(x => (x.EndDate - x.StartDate).TotalDays <= 30)
                .WithMessage("Bookings cannot exceed 30 days.");
        }
    }
}
