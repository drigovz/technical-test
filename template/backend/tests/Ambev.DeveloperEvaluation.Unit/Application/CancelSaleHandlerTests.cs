using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale ID When cancelling entire sale Then returns success response")]
    public async Task Handle_CancelEntireSale_ReturnsSuccessResponse()
    {
        var command = CancelSaleHandlerTestData.GenerateCancelEntireSaleCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-123",
            CustomerName = "John Doe",
            Items = new List<SaleItem>
            {
                new SaleItem { Id = Guid.NewGuid(), ProductName = "Product 1", Quantity = 2 },
                new SaleItem { Id = Guid.NewGuid(), ProductName = "Product 2", Quantity = 3 }
            }
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.IsCancelled.Should().BeTrue();
        existingSale.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid sale ID and item IDs When cancelling specific items Then returns success response")]
    public async Task Handle_CancelSpecificItems_ReturnsSuccessResponse()
    {
        var itemIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = CancelSaleHandlerTestData.GenerateCancelSpecificItemsCommand(2);
        command.ItemIds = itemIds;

        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-123",
            CustomerName = "John Doe",
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemIds[0], ProductName = "Product 1", Quantity = 2, UnitPrice = 10.00m },
                new SaleItem { Id = itemIds[1], ProductName = "Product 2", Quantity = 3, UnitPrice = 15.00m },
                new SaleItem { Id = Guid.NewGuid(), ProductName = "Product 3", Quantity = 1, UnitPrice = 5.00m }
            }
        };

        existingSale.UpdateTotalAmount();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.IsCancelled.Should().BeFalse();
        result.CancelledItemIds.Should().HaveCount(2);
        result.CancelledItemIds.Should().Contain(itemIds);
        existingSale.Items.Count(i => i.IsCancelled).Should().Be(2);
        existingSale.IsCancelled.Should().BeFalse();
        await _saleRepository.Received(1).UpdateAsync(existingSale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given all items cancelled When cancelling specific items Then cancels entire sale")]
    public async Task Handle_CancelAllSpecificItems_CancelsEntireSale()
    {
        var itemIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = CancelSaleHandlerTestData.GenerateCancelSpecificItemsCommand(2);
        command.ItemIds = itemIds;

        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-456",
            Items = new List<SaleItem>
            {
                new SaleItem { Id = itemIds[0], ProductName = "Product 1", Quantity = 2, UnitPrice = 10.00m },
                new SaleItem { Id = itemIds[1], ProductName = "Product 2", Quantity = 3, UnitPrice = 15.00m }
            }
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsCancelled.Should().BeTrue();
        existingSale.IsCancelled.Should().BeTrue();
        existingSale.Items.Should().OnlyContain(i => i.IsCancelled);
    }

    [Fact(DisplayName = "Given invalid cancellation data When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new CancelSaleCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent sale ID When cancelling sale Then throws exception")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = CancelSaleHandlerTestData.GenerateCancelEntireSaleCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling sale Then throws exception")]
    public async Task Handle_AlreadyCancelledSale_ThrowsException()
    {
        var command = CancelSaleHandlerTestData.GenerateCancelEntireSaleCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = "SALE-789",
            IsCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled");
    }
}
