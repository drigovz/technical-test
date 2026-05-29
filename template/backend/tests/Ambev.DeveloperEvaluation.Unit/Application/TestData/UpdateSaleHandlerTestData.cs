using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for UpdateSaleHandler using the Bogus library.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    private static readonly Faker<UpdateSaleCommand> UpdateSaleCommandFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => GenerateValidSaleItemDtos(f.Random.Number(1, 5)));

    private static readonly Faker<UpdateSaleItemDto> UpdateSaleItemDtoFaker = new Faker<UpdateSaleItemDto>()
        .RuleFor(i => i.Id, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100));

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return UpdateSaleCommandFaker.Generate();
    }

    public static List<UpdateSaleItemDto> GenerateValidSaleItemDtos(int count)
    {
        return UpdateSaleItemDtoFaker.Generate(count);
    }
}
