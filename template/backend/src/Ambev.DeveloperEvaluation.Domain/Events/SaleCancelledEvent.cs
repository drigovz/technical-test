using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent
{
    /// <summary>
    /// Gets the sale that was cancelled.
    /// </summary>
    public Sale Sale { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime EventDate { get; }

    /// <summary>
    /// Initializes a new instance of SaleCancelledEvent.
    /// </summary>
    /// <param name="sale">The sale that was cancelled.</param>
    public SaleCancelledEvent(Sale sale)
    {
        Sale = sale;
        EventDate = DateTime.UtcNow;
    }
}