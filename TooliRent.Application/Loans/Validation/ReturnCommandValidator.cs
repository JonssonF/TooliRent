using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Loans.Commands;

namespace TooliRent.Application.Loans.Validation
{
    public sealed class ReturnCommandValidator : AbstractValidator<ReturnCommand>
    {
        public ReturnCommandValidator() 
        {
            RuleFor(x => x.LoanId)
                .GreaterThan(0)
                .WithMessage("LoanId must be greater than 0.");
            RuleFor(x => x.LoanId)
                .NotEmpty()
                .WithMessage("LoanId is required.");
            RuleFor(x => x.LoanId)
                .NotNull()
                .WithMessage("LoanId cannot be null.");
        }
    }
}
