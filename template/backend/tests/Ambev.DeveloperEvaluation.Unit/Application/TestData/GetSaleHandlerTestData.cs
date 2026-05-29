using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for GetSaleHandler using the Bogus library.
/// </summary>
public static class GetSaleHandlerTestData
{
    private static readonly Faker<GetSaleCommand> GetSaleCommandFaker = new Faker<GetSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid());

    public static GetSaleCommand GenerateValidCommand()
    {
        return GetSaleCommandFaker.Generate();
    }
}
