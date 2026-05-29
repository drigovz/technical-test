using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
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
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateSaleHandler>>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateExistingSale(command);

        SetupMapperForItems(command);

        var result = new UpdateSaleResult
        {
            Id = existingSale.Id,
            SaleNumber = command.SaleNumber,
            TotalAmount = existingSale.TotalAmount
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>()).Returns(result);

        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        updateSaleResult.Should().NotBeNull();
        updateSaleResult.Id.Should().Be(command.Id);
        updateSaleResult.SaleNumber.Should().Be(command.SaleNumber);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new UpdateSaleCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given non-existent sale ID When updating sale Then throws exception")]
    public async Task Handle_NonExistentSale_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given cancelled sale When updating sale Then throws exception")]
    public async Task Handle_CancelledSale_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale
        {
            Id = command.Id,
            SaleNumber = command.SaleNumber,
            IsCancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale");
    }

    [Fact(DisplayName = "Given duplicate sale number When updating sale Then throws exception")]
    public async Task Handle_DuplicateSaleNumber_ThrowsException()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateExistingSale(command);
        existingSale.SaleNumber = "ORIGINAL-NUMBER";

        var otherSale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(otherSale);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    [Fact(DisplayName = "Given sale with items When updating sale Then applies discount to items")]
    public async Task Handle_ValidRequest_AppliesDiscountToItems()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateExistingSale(command);

        SetupMapperForItems(command);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        foreach (var item in existingSale.Items)
        {
            item.Discount.Should().BeGreaterOrEqualTo(0);
            item.TotalAmount.Should().Be(item.Quantity * item.UnitPrice - item.Discount);
        }
    }

    [Fact(DisplayName = "Given sale with items When updating sale Then replaces existing items")]
    public async Task Handle_ValidRequest_ReplacesExistingItems()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateExistingSale(command);
        existingSale.Items.Add(new SaleItem
        {
            Id = Guid.NewGuid(),
            ProductName = "Old Product",
            Quantity = 1,
            UnitPrice = 5.00m
        });

        SetupMapperForItems(command);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        existingSale.Items.Should().HaveCount(command.Items.Count);
        _mapper.Received(command.Items.Count).Map<SaleItem>(Arg.Any<UpdateSaleItemDto>());
    }

    [Fact(DisplayName = "Given valid command When handling Then maps command onto existing sale")]
    public async Task Handle_ValidRequest_MapsCommandOntoExistingSale()
    {
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = CreateExistingSale(command);

        SetupMapperForItems(command);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        _mapper.Received(1).Map(command, existingSale);
    }

    private static Sale CreateExistingSale(UpdateSaleCommand command)
    {
        return new Sale
        {
            Id = command.Id,
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>()
        };
    }

    private void SetupMapperForItems(UpdateSaleCommand command)
    {
        foreach (var itemDto in command.Items)
        {
            var item = new SaleItem
            {
                Id = itemDto.Id,
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };

            _mapper.Map<SaleItem>(itemDto).Returns(item);
        }
    }
}
