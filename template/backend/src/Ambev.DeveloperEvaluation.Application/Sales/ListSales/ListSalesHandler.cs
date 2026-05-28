using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests.
/// </summary>
public sealed class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of ListSalesHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSalesCommand request.
    /// </summary>
    /// <param name="command">The ListSales command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of sales.</returns>
    public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListSalesCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        IEnumerable<Sale> sales;

        if (command.CustomerId.HasValue)
        {
            sales = await _saleRepository.GetByCustomerIdAsync(command.CustomerId.Value, cancellationToken);
        }
        else if (command.BranchId.HasValue)
        {
            sales = await _saleRepository.GetByBranchIdAsync(command.BranchId.Value, cancellationToken);
        }
        else
        {
            sales = await _saleRepository.GetAllAsync(cancellationToken);
        }

        if (command.IsCancelled.HasValue)
        {
            sales = sales.Where(s => s.IsCancelled == command.IsCancelled.Value);
        }

        var result = new ListSalesResult
        {
            Sales = _mapper.Map<List<ListSaleItemResult>>(sales)
        };

        return result;
    }
}