using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Loans.Commands;

namespace TooliRent.Application.Loans.Validation
{
    public sealed class PickupCommandValidator : AbstractValidator<PickupCommand>
    {
        public PickupCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("BookingId must be greater than 0.");
        }
    }
}
