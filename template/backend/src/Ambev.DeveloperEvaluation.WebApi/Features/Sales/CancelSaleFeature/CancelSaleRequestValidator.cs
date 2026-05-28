using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleFeature;

/// <summary>
/// Validator for CancelSaleRequest.
/// </summary>
public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of CancelSaleRequestValidator.
    /// </summary>
    public CancelSaleRequestValidator()
    {
        RuleFor(request => request.SaleId)
            .NotEmpty().WithMessage("Sale ID is required");
    }
}