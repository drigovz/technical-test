using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Command for listing sales.
/// </summary>
public class ListSalesCommand : IRequest<ListSalesResult>
{
    /// <summary>
    /// Gets or sets the customer ID to filter by (optional).
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the branch ID to filter by (optional).
    /// </summary>
    public Guid? BranchId { get; set; }

    /// <summary>
    /// Gets or sets whether to include only cancelled sales (optional).
    /// </summary>
    public bool? IsCancelled { get; set; }

    /// <summary>
    /// Validates the command.
    /// </summary>
    /// <returns>Validation result.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new ListSalesCommandValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}