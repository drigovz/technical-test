using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleFeature;

/// <summary>
/// Validator for UpdateSaleRequest.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of UpdateSaleRequestValidator.
    /// </summary>
    public UpdateSaleRequestValidator()
    {
        RuleFor(request => request.Id)
            .NotEmpty().WithMessage("Sale ID is required");

        RuleFor(request => request.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required")
            .MaximumLength(50).WithMessage("Sale number must not exceed 50 characters");

        RuleFor(request => request.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");

        RuleFor(request => request.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Customer name must not exceed 100 characters");

        RuleFor(request => request.BranchId)
            .NotEmpty().WithMessage("Branch ID is required");

        RuleFor(request => request.BranchName)
            .NotEmpty().WithMessage("Branch name is required")
            .MaximumLength(100).WithMessage("Branch name must not exceed 100 characters");

        RuleFor(request => request.Items)
            .NotNull().WithMessage("Sale items are required")
            .Must(items => items.Count > 0).WithMessage("Sale must have at least one item");

        RuleForEach(request => request.Items).SetValidator(new UpdateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validator for UpdateSaleItemRequest.
/// </summary>
public class UpdateSaleItemRequestValidator : AbstractValidator<UpdateSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of UpdateSaleItemRequestValidator.
    /// </summary>
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(item => item.Id)
            .NotEmpty().WithMessage("Sale item ID is required");

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