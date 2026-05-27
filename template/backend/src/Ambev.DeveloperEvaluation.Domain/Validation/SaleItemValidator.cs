using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for SaleItem entity.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(160).WithMessage("Product name must not exceed 160 characters");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20 items");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        RuleFor(item => item.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to 0");

        RuleFor(item => item.TotalAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Total amount must be greater than or equal to 0");
    }
}