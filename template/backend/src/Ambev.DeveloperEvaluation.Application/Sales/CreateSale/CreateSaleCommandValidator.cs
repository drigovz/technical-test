using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleCommand.
/// </summary>
public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(command => command.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required")
            .MaximumLength(50).WithMessage("Sale number must not exceed 50 characters");

        RuleFor(command => command.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(command => command.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters");

        RuleFor(command => command.BranchId)
            .NotEmpty().WithMessage("Branch ID is required");

        RuleFor(command => command.BranchName)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(command => command.Items)
            .NotNull().WithMessage("Sale items are required")
            .Must(items => items.Count > 0).WithMessage("Sale must have at least one item");

        RuleForEach(command => command.Items).SetValidator(new CreateSaleItemDtoValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemDto.
/// </summary>
public class CreateSaleItemDtoValidator : AbstractValidator<CreateSaleItemDto>
{
    public CreateSaleItemDtoValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name must not exceed 100 characters");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThanOrEqualTo(20).WithMessage("Quantity cannot exceed 20 items");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");
    }
}