using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="ListSalesHandler"/> class.
/// </summary>
public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given no filters When listing sales Then returns all sales")]
    public async Task Handle_NoFilters_ReturnsAllSales()
    {
        var command = ListSalesHandlerTestData.GenerateEmptyFilterCommand();
        var sales = CreateSampleSales(3);

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);

        var mappedSales = sales.Select(s => new ListSaleItemResult { Id = s.Id, SaleNumber = s.SaleNumber }).ToList();
        _mapper.Map<List<ListSaleItemResult>>(Arg.Any<IEnumerable<Sale>>()).Returns(mappedSales);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sales.Should().HaveCount(3);
        await _saleRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().GetByCustomerIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().GetByBranchIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given customer filter When listing sales Then returns customer sales")]
    public async Task Handle_CustomerFilter_ReturnsCustomerSales()
    {
        var command = ListSalesHandlerTestData.GenerateCustomerFilterCommand();
        var sales = CreateSampleSales(2);

        _saleRepository.GetByCustomerIdAsync(command.CustomerId!.Value, Arg.Any<CancellationToken>())
            .Returns(sales);

        _mapper.Map<List<ListSaleItemResult>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(sales.Select(s => new ListSaleItemResult { Id = s.Id }).ToList());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sales.Should().HaveCount(2);
        await _saleRepository.Received(1).GetByCustomerIdAsync(command.CustomerId!.Value, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given branch filter When listing sales Then returns branch sales")]
    public async Task Handle_BranchFilter_ReturnsBranchSales()
    {
        var command = ListSalesHandlerTestData.GenerateBranchFilterCommand();
        var sales = CreateSampleSales(2);

        _saleRepository.GetByBranchIdAsync(command.BranchId!.Value, Arg.Any<CancellationToken>())
            .Returns(sales);

        _mapper.Map<List<ListSaleItemResult>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(sales.Select(s => new ListSaleItemResult { Id = s.Id }).ToList());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sales.Should().HaveCount(2);
        await _saleRepository.Received(1).GetByBranchIdAsync(command.BranchId!.Value, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given customer and branch filters When listing sales Then prioritizes customer filter")]
    public async Task Handle_CustomerAndBranchFilters_PrioritizesCustomerFilter()
    {
        var command = new ListSalesCommand
        {
            CustomerId = Guid.NewGuid(),
            BranchId = Guid.NewGuid()
        };

        var sales = CreateSampleSales(1);

        _saleRepository.GetByCustomerIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(sales);

        _mapper.Map<List<ListSaleItemResult>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(new List<ListSaleItemResult> { new() { Id = sales.First().Id } });

        await _handler.Handle(command, CancellationToken.None);

        await _saleRepository.Received(1).GetByCustomerIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>());
        await _saleRepository.DidNotReceive().GetByBranchIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cancelled filter When listing sales Then returns only matching sales")]
    public async Task Handle_CancelledFilter_ReturnsFilteredSales()
    {
        var command = ListSalesHandlerTestData.GenerateCancelledFilterCommand(isCancelled: true);
        var sales = new List<Sale>
        {
            new() { Id = Guid.NewGuid(), SaleNumber = "S1", IsCancelled = true },
            new() { Id = Guid.NewGuid(), SaleNumber = "S2", IsCancelled = false },
            new() { Id = Guid.NewGuid(), SaleNumber = "S3", IsCancelled = true }
        };

        _saleRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(sales);

        _mapper.Map<List<ListSaleItemResult>>(Arg.Any<IEnumerable<Sale>>())
            .Returns(callInfo =>
            {
                var filtered = callInfo.Arg<IEnumerable<Sale>>().ToList();
                return filtered.Select(s => new ListSaleItemResult { Id = s.Id, IsCancelled = s.IsCancelled }).ToList();
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Sales.Should().HaveCount(2);
        result.Sales.Should().OnlyContain(s => s.IsCancelled);
    }

    private static List<Sale> CreateSampleSales(int count)
    {
        return Enumerable.Range(0, count)
            .Select(i => new Sale
            {
                Id = Guid.NewGuid(),
                SaleNumber = $"SALE-{i}",
                CustomerName = $"Customer {i}",
                TotalAmount = 100 * (i + 1)
            })
            .ToList();
    }
}
