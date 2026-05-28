namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleFeature;

/// <summary>
/// Request DTO for cancelling a sale.
/// </summary>
public class CancelSaleRequest
{
    /// <summary>
    /// Gets or sets the sale ID.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the item IDs to cancel (optional - if empty, cancels entire sale).
    /// </summary>
    public List<Guid> ItemIds { get; set; } = new List<Guid>();
}