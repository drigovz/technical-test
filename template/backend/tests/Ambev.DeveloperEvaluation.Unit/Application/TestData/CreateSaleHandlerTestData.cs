using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CreateSaleHandler using the Bogus library.
/// This class centralizes all test data generation to ensure consistency across test cases.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateSaleCommand entities.
    /// The generated commands will have valid:
    /// - SaleNumber (alphanumeric format)
    /// - SaleDate (recent dates)
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - Items (collection of valid CreateSaleItemDto)
    /// </summary>
    private static readonly Faker<CreateSaleCommand> CreateSaleCommandFaker = new Faker<CreateSaleCommand>()
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => GenerateValidSaleItemDtos(f.Random.Number(1, 5)));

    /// <summary>
    /// Configures the Faker to generate valid CreateSaleItemDto entities.
    /// The generated sale item DTOs will have valid:
    /// - ProductId and ProductName
    /// - Quantity (positive values)
    /// - UnitPrice (positive values)
    /// </summary>
    private static readonly Faker<CreateSaleItemDto> CreateSaleItemDtoFaker = new Faker<CreateSaleItemDto>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100));

    /// <summary>
    /// Generates a valid CreateSaleCommand entity with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CreateSaleCommand entity with randomly generated data.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return CreateSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a collection of valid CreateSaleItemDto entities.
    /// </summary>
    /// <param name="count">The number of sale item DTOs to generate.</param>
    /// <returns>A collection of valid CreateSaleItemDto entities.</returns>
    public static List<CreateSaleItemDto> GenerateValidSaleItemDtos(int count)
    {
        return CreateSaleItemDtoFaker.Generate(count);
    }

    /// <summary>
    /// Generates a valid sale number for testing.
    /// </summary>
    /// <returns>A valid sale number.</returns>
    public static string GenerateValidSaleNumber()
    {
        return new Faker().Random.AlphaNumeric(10);
    }

    /// <summary>
    /// Generates a valid customer name for testing.
    /// </summary>
    /// <returns>A valid customer name.</returns>
    public static string GenerateValidCustomerName()
    {
        return new Faker().Name.FullName();
    }

    /// <summary>
    /// Generates a valid branch name for testing.
    /// </summary>
    /// <returns>A valid branch name.</returns>
    public static string GenerateValidBranchName()
    {
        return new Faker().Company.CompanyName();
    }
}