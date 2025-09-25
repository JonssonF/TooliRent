using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Domain.Tools;

namespace TooliRent.Application.Tools.Validation
{
    public sealed class ToolCreateRequestValidator : AbstractValidator<ToolCreateRequest>
    {
        public ToolCreateRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.CategoryId).GreaterThan(0);
            RuleFor(x => x.PricePerDay).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Manufacturer).MaximumLength(100).When(x => x.Manufacturer is not null);
            RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
            RuleFor(x => x.SerialNumber).MaximumLength(100).When(x => x.SerialNumber is not null);
        }
    }
}
