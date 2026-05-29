using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid sale ID When getting sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-001",
            CustomerName = "Jane Doe",
            TotalAmount = 150.00m
        };

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            CustomerName = sale.CustomerName,
            TotalAmount = sale.TotalAmount
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(sale.Id);
        getSaleResult.SaleNumber.Should().Be(sale.SaleNumber);
        getSaleResult.TotalAmount.Should().Be(sale.TotalAmount);
    }

    [Fact(DisplayName = "Given invalid sale ID When getting sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new GetSaleCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent sale ID When getting sale Then throws exception")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given valid sale When getting sale Then maps entity to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-002",
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Product A",
                    Quantity = 5,
                    UnitPrice = 10.00m
                }
            }
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(new GetSaleResult { Id = sale.Id });

        await _handler.Handle(command, CancellationToken.None);

        _mapper.Received(1).Map<GetSaleResult>(sale);
    }
}
