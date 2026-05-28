namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleFeature;

/// <summary>
/// Request DTO for updating a sale.
/// </summary>
public class UpdateSaleRequest
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
    /// Gets or sets the customer ID.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the customer name (denormalized for reference).
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch ID where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the branch name (denormalized for reference).
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of sale items.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new List<UpdateSaleItemRequest>();
}

/// <summary>
/// Request DTO for updating sale items.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the sale item ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product ID.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product name (denormalized for reference).
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}