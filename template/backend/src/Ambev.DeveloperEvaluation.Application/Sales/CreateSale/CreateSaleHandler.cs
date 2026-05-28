using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests.
/// </summary>
public sealed class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger.</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request.
    /// </summary>
    /// <param name="command">The CreateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details.</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Check if sale number already exists
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        // Map command to sale entity
        var sale = _mapper.Map<Sale>(command);
        sale.Id = Guid.NewGuid();

        // Process each item and apply business rules
        foreach (var itemDto in command.Items)
        {
            var item = _mapper.Map<SaleItem>(itemDto);
            item.Id = Guid.NewGuid();
            item.SaleId = sale.Id;

            // Apply discount based on business rules
            item.ApplyDiscount();

            sale.Items.Add(item);
        }

        // Calculate total amount
        sale.UpdateTotalAmount();

        // Create the sale
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publish SaleCreated event (logging instead of actual message broker)
        var saleCreatedEvent = new SaleCreatedEvent(createdSale);
        _logger.LogInformation("SaleCreated event published: SaleNumber={SaleNumber}, TotalAmount={TotalAmount}, Customer={CustomerName}",
            saleCreatedEvent.Sale.SaleNumber, saleCreatedEvent.Sale.TotalAmount, saleCreatedEvent.Sale.CustomerName);

        // Map result
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}