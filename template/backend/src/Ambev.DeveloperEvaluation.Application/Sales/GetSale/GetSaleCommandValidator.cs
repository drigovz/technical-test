using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Validator for GetSaleCommand.
/// </summary>
public class GetSaleCommandValidator : AbstractValidator<GetSaleCommand>
{
    public GetSaleCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Sale ID is required");
    }
}