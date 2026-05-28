namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleFeature;

/// <summary>
/// Response DTO for cancelling a sale.
/// </summary>
public class CancelSaleResponse
{
    /// <summary>
    /// Gets or sets the sale ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the cancelled item IDs (if partial cancellation).
    /// </summary>
    public List<Guid> CancelledItemIds { get; set; } = new List<Guid>();
}