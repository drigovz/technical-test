using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for Sale and SaleItem entities using the Bogus library.
/// This class centralizes all test data generation to ensure consistency across test cases.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Sale entities.
    /// The generated sales will have valid:
    /// - SaleNumber (alphanumeric format)
    /// - SaleDate (recent dates)
    /// - CustomerId and CustomerName
    /// - BranchId and BranchName
    /// - TotalAmount (positive values)
    /// - Items (collection of valid SaleItems)
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .RuleFor(s => s.SaleNumber, f => f.Random.AlphaNumeric(10))
        .RuleFor(s => s.SaleDate, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Name.FullName())
        .RuleFor(s => s.BranchId, f => f.Random.Guid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(10, 1000))
        .RuleFor(s => s.Items, f => GenerateValidSaleItems(f.Random.Number(1, 5)));

    /// <summary>
    /// Configures the Faker to generate valid SaleItem entities.
    /// The generated sale items will have valid:
    /// - SaleId and ProductId
    /// - ProductName
    /// - Quantity (positive values)
    /// - UnitPrice (positive values)
    /// - Discount (non-negative values)
    /// - TotalAmount (calculated based on quantity, price, and discount)
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.SaleId, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1, 100))
        .RuleFor(i => i.Discount, f => f.Random.Decimal(0, 10))
        .RuleFor(i => i.TotalAmount, (f, i) => (i.Quantity * i.UnitPrice) - i.Discount);

    /// <summary>
    /// Generates a valid Sale entity with randomized data.
    /// The generated sale will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid Sale entity with randomly generated data.</returns>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Generates a valid SaleItem entity with randomized data.
    /// The generated sale item will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid SaleItem entity with randomly generated data.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        return SaleItemFaker.Generate();
    }

    /// <summary>
    /// Generates a collection of valid SaleItem entities.
    /// </summary>
    /// <param name="count">The number of sale items to generate.</param>
    /// <returns>A collection of valid SaleItem entities.</returns>
    public static ICollection<SaleItem> GenerateValidSaleItems(int count)
    {
        return SaleItemFaker.Generate(count);
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

    /// <summary>
    /// Generates a valid product name for testing.
    /// </summary>
    /// <returns>A valid product name.</returns>
    public static string GenerateValidProductName()
    {
        return new Faker().Commerce.ProductName();
    }

    /// <summary>
    /// Generates an invalid sale number (empty string) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid sale number.</returns>
    public static string GenerateInvalidSaleNumber()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates an invalid customer name (empty string) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid customer name.</returns>
    public static string GenerateInvalidCustomerName()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates an invalid branch name (empty string) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid branch name.</returns>
    public static string GenerateInvalidBranchName()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates an invalid product name (empty string) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid product name.</returns>
    public static string GenerateInvalidProductName()
    {
        return string.Empty;
    }

    /// <summary>
    /// Generates an invalid quantity (zero) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid quantity.</returns>
    public static int GenerateInvalidQuantity()
    {
        return 0;
    }

    /// <summary>
    /// Generates an invalid unit price (negative value) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid unit price.</returns>
    public static decimal GenerateInvalidUnitPrice()
    {
        return -1;
    }

    /// <summary>
    /// Generates an invalid discount (negative value) for testing negative scenarios.
    /// </summary>
    /// <returns>An invalid discount.</returns>
    public static decimal GenerateInvalidDiscount()
    {
        return -1;
    }
}