using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Application.Categories.DTOs;

namespace TooliRent.Application.Categories.Validation
{
    public sealed class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
    {
        public CategoryUpdateRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
            RuleFor(x => x)
                .Must(x => !string.Equals(x.Name?.Trim(), x.Description?.Trim(), StringComparison.OrdinalIgnoreCase))
                .WithMessage("Description must not be the same as the name.");
            RuleFor(x => x.Name)
                .Must(name => !string.Equals(name?.Trim(), "Uncategorized", StringComparison.OrdinalIgnoreCase))
                .WithMessage("The category name 'Uncategorized' is reserved and cannot be used.");
            RuleFor(x => x.Name)
                .Must(name => !string.Equals(name?.Trim(), "Category", StringComparison.OrdinalIgnoreCase))
                .WithMessage("The category name 'Category' is reserved and cannot be used.");
        }
    }
}
