using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateSaleHandler>>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateEmptySaleFromCommand(command);

        SetupMapperForItems(command, sale);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        _mapper.Map<Sale>(command).Returns(sale);
        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(callInfo =>
        {
            var createdSale = callInfo.Arg<Sale>();
            return new CreateSaleResult
            {
                Id = createdSale.Id,
                SaleNumber = createdSale.SaleNumber,
                TotalAmount = createdSale.TotalAmount
            };
        });

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        createSaleResult.Should().NotBeNull();
        createSaleResult.SaleNumber.Should().Be(command.SaleNumber);
        createSaleResult.TotalAmount.Should().BeGreaterThan(0);
        sale.Items.Should().HaveCount(command.Items.Count);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var command = new CreateSaleCommand();

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given duplicate sale number When creating sale Then throws exception")]
    public async Task Handle_DuplicateSaleNumber_ThrowsException()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale { SaleNumber = command.SaleNumber };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    [Fact(DisplayName = "Given valid command When handling Then maps command to sale entity")]
    public async Task Handle_ValidRequest_MapsCommandToSale()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateEmptySaleFromCommand(command);

        SetupMapperForItems(command, sale);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        _mapper.Received(1).Map<Sale>(Arg.Is<CreateSaleCommand>(c =>
            c.SaleNumber == command.SaleNumber &&
            c.CustomerName == command.CustomerName &&
            c.BranchName == command.BranchName));
    }

    [Fact(DisplayName = "Given sale with items When creating sale Then applies discount to items")]
    public async Task Handle_ValidRequest_AppliesDiscountToItems()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateEmptySaleFromCommand(command);

        SetupMapperForItems(command, sale);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        foreach (var item in sale.Items)
        {
            item.Discount.Should().BeGreaterOrEqualTo(0);
            item.TotalAmount.Should().Be(item.Quantity * item.UnitPrice - item.Discount);
        }
    }

    [Fact(DisplayName = "Given sale with items When creating sale Then calculates total amount correctly")]
    public async Task Handle_ValidRequest_CalculatesTotalAmount()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateEmptySaleFromCommand(command);

        SetupMapperForItems(command, sale);

        decimal expectedTotal = 0;
        foreach (var itemDto in command.Items)
        {
            var item = new SaleItem
            {
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };
            item.ApplyDiscount();
            expectedTotal += item.TotalAmount;
        }

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        sale.TotalAmount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Given sale items When creating sale Then maps each item DTO to entity")]
    public async Task Handle_ValidRequest_MapsEachItemDtoToEntity()
    {
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var sale = CreateEmptySaleFromCommand(command);

        SetupMapperForItems(command, sale);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Sale?>(null));

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        await _handler.Handle(command, CancellationToken.None);

        _mapper.Received(command.Items.Count).Map<SaleItem>(Arg.Any<CreateSaleItemDto>());
    }

    private static Sale CreateEmptySaleFromCommand(CreateSaleCommand command)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            CustomerId = command.CustomerId,
            CustomerName = command.CustomerName,
            BranchId = command.BranchId,
            BranchName = command.BranchName,
            Items = new List<SaleItem>()
        };
    }

    private void SetupMapperForItems(CreateSaleCommand command, Sale sale)
    {
        foreach (var itemDto in command.Items)
        {
            var item = new SaleItem
            {
                ProductId = itemDto.ProductId,
                ProductName = itemDto.ProductName,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };

            _mapper.Map<SaleItem>(itemDto).Returns(item);
        }
    }
}
