using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is created.
/// </summary>
public class SaleCreatedEvent
{
    /// <summary>
    /// Gets the sale that was created.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime EventDate { get; }

    /// <summary>
    /// Initializes a new instance of SaleCreatedEvent.
    /// </summary>
    /// <param name="sale">The sale that was created.</param>
    public SaleCreatedEvent(Sale sale)
    {
        Sale = sale;
        EventDate = DateTime.UtcNow;
    }
}