using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data for CancelSaleHandler using the Bogus library.
/// This class centralizes all test data generation to ensure consistency across test cases.
/// </summary>
public static class CancelSaleHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CancelSaleCommand entities.
    /// The generated commands will have valid:
    /// - Id (valid GUID)
    /// - ItemIds (collection of valid GUIDs)
    /// </summary>
    private static readonly Faker<CancelSaleCommand> CancelSaleCommandFaker = new Faker<CancelSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.ItemIds, f => GenerateValidItemIds(f.Random.Number(0, 3)));

    /// <summary>
    /// Generates a valid CancelSaleCommand entity with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CancelSaleCommand entity with randomly generated data.</returns>
    public static CancelSaleCommand GenerateValidCommand()
    {
        return CancelSaleCommandFaker.Generate();
    }

    /// <summary>
    /// Generates a CancelSaleCommand for cancelling an entire sale (no item IDs).
    /// </summary>
    /// <returns>A CancelSaleCommand with empty ItemIds collection.</returns>
    public static CancelSaleCommand GenerateCancelEntireSaleCommand()
    {
        return new CancelSaleCommand
        {
            Id = Guid.NewGuid(),
            ItemIds = new List<Guid>()
        };
    }

    /// <summary>
    /// Generates a CancelSaleCommand for cancelling specific items.
    /// </summary>
    /// <param name="itemCount">The number of item IDs to include.</param>
    /// <returns>A CancelSaleCommand with specified number of item IDs.</returns>
    public static CancelSaleCommand GenerateCancelSpecificItemsCommand(int itemCount)
    {
        return new CancelSaleCommand
        {
            Id = Guid.NewGuid(),
            ItemIds = GenerateValidItemIds(itemCount)
        };
    }

    /// <summary>
    /// Generates a collection of valid item IDs.
    /// </summary>
    /// <param name="count">The number of item IDs to generate.</param>
    /// <returns>A collection of valid GUIDs.</returns>
    private static List<Guid> GenerateValidItemIds(int count)
    {
        var ids = new List<Guid>();
        for (int i = 0; i < count; i++)
        {
            ids.Add(Guid.NewGuid());
        }
        return ids;
    }
}