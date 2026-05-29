using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover cancellation, total amount calculation, and validation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that when a sale is cancelled, the IsCancelled property is set to true.
    /// </summary>
    [Fact(DisplayName = "Sale should be cancelled successfully")]
    public void Given_ActiveSale_When_Cancelled_Then_IsCancelledShouldBeTrue()
    {
        var sale = SaleTestData.GenerateValidSale();

        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that attempting to cancel an already cancelled sale throws an exception.
    /// </summary>
    [Fact(DisplayName = "Cancelling already cancelled sale should throw exception")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ShouldThrowException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var exception = Assert.Throws<InvalidOperationException>(sale.Cancel);
        Assert.Equal("Sale is already cancelled", exception.Message);
    }

    /// <summary>
    /// Tests that the total amount is correctly calculated based on sale items.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly from sale items")]
    public void Given_SaleWithItems_When_UpdateTotalAmount_Then_TotalShouldBeSumOfItems()
    {
        var sale = SaleTestData.GenerateValidSale();
        var expectedTotal = sale.Items.Sum(item => item.TotalAmount);

        sale.UpdateTotalAmount();

        Assert.Equal(expectedTotal, sale.TotalAmount);
        Assert.NotNull(sale.UpdatedAt);
    }

    /// <summary>
    /// Tests that validation passes when all sale properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        var sale = SaleTestData.GenerateValidSale();

        var result = sale.Validate();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale data")]
    public void Given_InvalidSaleData_When_Validated_Then_ShouldReturnInvalid()
    {
        var sale = new Sale
        {
            SaleNumber = SaleTestData.GenerateInvalidSaleNumber(), // Invalid: empty
            CustomerName = SaleTestData.GenerateInvalidCustomerName(), // Invalid: empty
            BranchName = SaleTestData.GenerateInvalidBranchName(), // Invalid: empty
            TotalAmount = -1, // Invalid: negative amount
            Items = new List<SaleItem>() // Empty items collection
        };

        var result = sale.Validate();

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that the sale constructor initializes CreatedAt to current UTC time.
    /// </summary>
    [Fact(DisplayName = "Sale constructor should initialize CreatedAt to current UTC time")]
    public void Given_NewSale_When_Created_Then_CreatedAtShouldBeSet()
    {
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        var sale = new Sale();

        Assert.True(sale.CreatedAt >= beforeCreation);
        Assert.True(sale.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    /// <summary>
    /// Tests that UpdateTotalAmount updates the UpdatedAt timestamp.
    /// </summary>
    [Fact(DisplayName = "UpdateTotalAmount should update the UpdatedAt timestamp")]
    public void Given_Sale_When_UpdateTotalAmount_Then_UpdatedAtShouldBeSet()
    {
        var sale = SaleTestData.GenerateValidSale();
        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        sale.UpdateTotalAmount();

        Assert.True(sale.UpdatedAt >= beforeUpdate);
        Assert.True(sale.UpdatedAt <= DateTime.UtcNow.AddSeconds(1));
    }
}