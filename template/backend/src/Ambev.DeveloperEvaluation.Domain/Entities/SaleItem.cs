using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale record.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale ID this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }

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

    /// <summary>
    /// Gets or sets the discount applied to this item.
    /// </summary>
    public decimal Discount { get; set; } = 0;

    /// <summary>
    /// Gets or sets the total amount for this item (quantity * unit price - discount).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets whether this item is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; } = false;

    /// <summary>
    /// Gets the date and time when the sale item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale item.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the SaleItem class.
    /// </summary>
    public SaleItem()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels this sale item.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Sale item is already cancelled");

        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total amount based on quantity, unit price, and discount.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = (Quantity * UnitPrice) - Discount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Applies discount based on business rules.
    /// </summary>
    public void ApplyDiscount()
    {
        Discount = Quantity switch
        {
            // Apply business rules for discounts
            >= 10 and <= 20 => (Quantity * UnitPrice) * 0.20m,
            >= 4 and < 10 => (Quantity * UnitPrice) * 0.10m,
            < 4 => 0,
            _ => throw new InvalidOperationException("Cannot sell more than 20 identical items")
        };

        CalculateTotalAmount();
    }

    /// <summary>
    /// Performs validation of the sale item entity.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing validation results.
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}