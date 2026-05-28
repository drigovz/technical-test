using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Handler for processing CancelSaleCommand requests.
/// </summary>
public sealed class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CancelSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger.</param>
    public CancelSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CancelSaleCommand request.
    /// </summary>
    /// <param name="command">The CancelSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancellation result.</returns>
    public async Task<CancelSaleResult> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        var result = new CancelSaleResult
        {
            Id = existingSale.Id,
            SaleNumber = existingSale.SaleNumber
        };

        if (command.ItemIds == null || command.ItemIds.Count == 0)
        {
            existingSale.Cancel();
            result.IsCancelled = true;

            // Publish SaleCancelled event (logging instead of actual message broker)
            var saleCancelledEvent = new SaleCancelledEvent(existingSale);
            _logger.LogInformation("SaleCancelled event published: SaleNumber={SaleNumber}, Customer={CustomerName}",
                saleCancelledEvent.Sale.SaleNumber, saleCancelledEvent.Sale.CustomerName);
        }
        else
        {
            foreach (var itemId in command.ItemIds)
            {
                var item = existingSale.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    item.Cancel();
                    result.CancelledItemIds.Add(item.Id);

                    // Publish ItemCancelled event (logging instead of actual message broker)
                    var itemCancelledEvent = new ItemCancelledEvent(item, existingSale);
                    _logger.LogInformation("ItemCancelled event published: SaleNumber={SaleNumber}, Product={ProductName}, Quantity={Quantity}",
                        itemCancelledEvent.Sale.SaleNumber, itemCancelledEvent.SaleItem.ProductName, itemCancelledEvent.SaleItem.Quantity);
                }
            }

            // If all items are cancelled, cancel the entire sale
            if (existingSale.Items.All(i => i.IsCancelled))
            {
                existingSale.Cancel();
                result.IsCancelled = true;
            }
            else
            {
                existingSale.UpdateTotalAmount();
            }
        }

        await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        return result;
    }
}