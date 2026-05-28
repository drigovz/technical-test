using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is modified.
/// </summary>
public class SaleModifiedEvent
{
    /// <summary>
    /// Gets the sale that was modified.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime EventDate { get; }

    /// <summary>
    /// Initializes a new instance of SaleModifiedEvent.
    /// </summary>
    /// <param name="sale">The sale that was modified.</param>
    public SaleModifiedEvent(Sale sale)
    {
        Sale = sale;
        EventDate = DateTime.UtcNow;
    }
}