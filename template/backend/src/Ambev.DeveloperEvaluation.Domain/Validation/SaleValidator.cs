using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for Sale entity.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required")
            .MaximumLength(50).WithMessage("Sale number must not exceed 50 characters");

        RuleFor(sale => sale.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters");

        RuleFor(sale => sale.BranchId)
            .NotEmpty().WithMessage("Branch ID is required");

        RuleFor(sale => sale.BranchName)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(sale => sale.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount must be greater than or equal to 0");

        RuleFor(sale => sale.Items)
            .NotNull().WithMessage("Sale items are required")
            .Must(items => items.Count > 0).WithMessage("Sale must have at least one item");
    }
}