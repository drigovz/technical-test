using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale item is cancelled.
/// </summary>
public class ItemCancelledEvent
{
    /// <summary>
    /// Gets the sale item that was cancelled.
    /// </summary>
    public SaleItem SaleItem { get; }

    /// <summary>
    /// Gets the sale that contains the cancelled item.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime EventDate { get; }

    /// <summary>
    /// Initializes a new instance of ItemCancelledEvent.
    /// </summary>
    /// <param name="saleItem">The sale item that was cancelled.</param>
    /// <param name="sale">The sale that contains the cancelled item.</param>
    public ItemCancelledEvent(SaleItem saleItem, Sale sale)
    {
        SaleItem = saleItem;
        Sale = sale;
        EventDate = DateTime.UtcNow;
    }
}