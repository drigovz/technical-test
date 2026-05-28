using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests.
/// </summary>
public sealed class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger.</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request.
    /// </summary>
    /// <param name="command">The UpdateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale details.</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        if (existingSale.SaleNumber != command.SaleNumber)
        {
            var saleWithSameNumber = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
            if (saleWithSameNumber != null && saleWithSameNumber.Id != command.Id)
                throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");
        }

        _mapper.Map(command, existingSale);
        existingSale.Items.Clear();

        foreach (var itemDto in command.Items)
        {
            var item = _mapper.Map<SaleItem>(itemDto);
            item.SaleId = existingSale.Id;

            item.ApplyDiscount();

            existingSale.Items.Add(item);
        }

        existingSale.UpdateTotalAmount();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        var saleModifiedEvent = new SaleModifiedEvent(updatedSale);
        _logger.LogInformation("SaleModified event published: SaleNumber={SaleNumber}, TotalAmount={TotalAmount}, Customer={CustomerName}",
            saleModifiedEvent.Sale.SaleNumber, saleModifiedEvent.Sale.TotalAmount, saleModifiedEvent.Sale.CustomerName);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }
}