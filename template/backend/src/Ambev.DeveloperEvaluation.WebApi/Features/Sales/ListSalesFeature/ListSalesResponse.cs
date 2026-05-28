namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSalesFeature;

/// <summary>
/// Response DTO for listing sales.
/// </summary>
public class ListSalesResponse
{
    /// <summary>
    /// Gets or sets the collection of sales.
    /// </summary>
    public List<ListSaleItemResponse> Sales { get; set; } = new List<ListSaleItemResponse>();
}

/// <summary>
/// Response DTO for individual sale in the list.
/// </summary>
public class ListSaleItemResponse
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
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total sale amount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}