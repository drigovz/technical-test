using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Validator for ListSalesCommand.
/// </summary>
public class ListSalesCommandValidator : AbstractValidator<ListSalesCommand>
{
    public ListSalesCommandValidator()
    { }
}