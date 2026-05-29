using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for ListSalesHandler using the Bogus library.
/// </summary>
public static class ListSalesHandlerTestData
{
    public static ListSalesCommand GenerateEmptyFilterCommand()
    {
        return new ListSalesCommand();
    }

    public static ListSalesCommand GenerateCustomerFilterCommand()
    {
        return new Faker<ListSalesCommand>()
            .RuleFor(c => c.CustomerId, f => f.Random.Guid())
            .Generate();
    }

    public static ListSalesCommand GenerateBranchFilterCommand()
    {
        return new Faker<ListSalesCommand>()
            .RuleFor(c => c.BranchId, f => f.Random.Guid())
            .Generate();
    }

    public static ListSalesCommand GenerateCancelledFilterCommand(bool isCancelled)
    {
        return new ListSalesCommand { IsCancelled = isCancelled };
    }
}
