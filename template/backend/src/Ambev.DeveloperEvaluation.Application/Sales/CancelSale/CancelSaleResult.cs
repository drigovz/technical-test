namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Result returned after cancelling a sale.
/// </summary>
public class CancelSaleResult
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
    /// Gets or sets the cancelled item IDs.
    /// </summary>
    public List<Guid> CancelledItemIds { get; set; } = new List<Guid>();
}