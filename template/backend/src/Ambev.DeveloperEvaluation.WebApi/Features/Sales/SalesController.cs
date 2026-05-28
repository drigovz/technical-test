using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSalesFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleFeature;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleFeature;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for sales operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of SalesController.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">The create sale request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetSale), new { id = response.Id }, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Gets a sale by ID.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale details.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSaleCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetSaleResponse>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = _mapper.Map<GetSaleResponse>(response)
        });
    }

    /// <summary>
    /// Lists sales with optional filters.
    /// </summary>
    /// <param name="customerId">Optional customer ID filter.</param>
    /// <param name="branchId">Optional branch ID filter.</param>
    /// <param name="isCancelled">Optional cancelled status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of sales.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<ListSalesResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSales([FromQuery] Guid? customerId, [FromQuery] Guid? branchId, [FromQuery] bool? isCancelled, CancellationToken cancellationToken)
    {
        var command = new ListSalesCommand
        {
            CustomerId = customerId,
            BranchId = branchId,
            IsCancelled = isCancelled
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<ListSalesResponse>
        {
            Success = true,
            Message = "Sales retrieved successfully",
            Data = _mapper.Map<ListSalesResponse>(response)
        });
    }

    /// <summary>
    /// Updates a sale.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="request">The update sale request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale(Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest(new ApiResponse { Success = false, Message = "Sale ID mismatch" });

        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateSaleResponse>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = _mapper.Map<UpdateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Cancels a sale or specific sale items.
    /// </summary>
    /// <param name="id">The sale ID.</param>
    /// <param name="request">The cancel sale request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cancellation result.</returns>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale(Guid id, [FromBody] CancelSaleRequest request, CancellationToken cancellationToken)
    {
        if (id != request.SaleId)
            return BadRequest(new ApiResponse { Success = false, Message = "Sale ID mismatch" });

        var validator = new CancelSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CancelSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelSaleResponse>
        {
            Success = true,
            Message = response.IsCancelled ? "Sale cancelled successfully" : "Sale items cancelled successfully",
            Data = _mapper.Map<CancelSaleResponse>(response)
        });
    }
}