using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover cancellation, discount application, total amount calculation, and validation scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that when a sale item is cancelled, the IsCancelled property is set to true.
    /// </summary>
    [Fact(DisplayName = "Sale item should be cancelled successfully")]
    public void Given_ActiveSaleItem_When_Cancelled_Then_IsCancelledShouldBeTrue()
    {
        var saleItem = SaleTestData.GenerateValidSaleItem();
        
        saleItem.Cancel();

        Assert.True(saleItem.IsCancelled);
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that attempting to cancel an already cancelled sale item throws an exception.
    /// </summary>
    [Fact(DisplayName = "Cancelling already cancelled sale item should throw exception")]
    public void Given_CancelledSaleItem_When_CancelledAgain_Then_ShouldThrowException()
    {
        var saleItem = SaleTestData.GenerateValidSaleItem();
        saleItem.Cancel();

        var exception = Assert.Throws<InvalidOperationException>(saleItem.Cancel);
        Assert.Equal("Sale item is already cancelled", exception.Message);
    }

    /// <summary>
    /// Tests that the total amount is correctly calculated based on quantity, unit price, and discount.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly from quantity, price, and discount")]
    public void Given_SaleItemWithValues_When_CalculateTotalAmount_Then_TotalShouldBeCorrect()
    {
        var saleItem = new SaleItem
        {
            Quantity = 5,
            UnitPrice = 10.00m,
            Discount = 2.50m
        };

        saleItem.CalculateTotalAmount();

        Assert.Equal(47.50m, saleItem.TotalAmount);
        Assert.NotNull(saleItem.UpdatedAt);
    }

    /// <summary>
    /// Tests that discount is applied correctly based on quantity business rules.
    /// </summary>
    [Fact(DisplayName = "Discount should be applied correctly based on quantity")]
    public void Given_SaleItemWithQuantity_When_ApplyDiscount_Then_DiscountShouldBeCorrect()
    {
        // Test case 1: Quantity < 4 (no discount)
        var saleItem1 = new SaleItem
        {
            Quantity = 3,
            UnitPrice = 10.00m
        };
        saleItem1.ApplyDiscount();
        Assert.Equal(0, saleItem1.Discount);
        Assert.Equal(30.00m, saleItem1.TotalAmount);

        // Test case 2: Quantity between 4 and 9 (10% discount)
        var saleItem2 = new SaleItem
        {
            Quantity = 5,
            UnitPrice = 10.00m
        };
        saleItem2.ApplyDiscount();
        Assert.Equal(5.00m, saleItem2.Discount); // 10% of 50.00
        Assert.Equal(45.00m, saleItem2.TotalAmount);

        // Test case 3: Quantity between 10 and 20 (20% discount)
        var saleItem3 = new SaleItem
        {
            Quantity = 15,
            UnitPrice = 10.00m
        };
        saleItem3.ApplyDiscount();
        Assert.Equal(30.00m, saleItem3.Discount); // 20% of 150.00
        Assert.Equal(120.00m, saleItem3.TotalAmount);
    }

    /// <summary>
    /// Tests that applying discount to quantity > 20 throws an exception.
    /// </summary>
    [Fact(DisplayName = "Applying discount to quantity over 20 should throw exception")]
    public void Given_SaleItemWithQuantityOver20_When_ApplyDiscount_Then_ShouldThrowException()
    {
        var saleItem = new SaleItem
        {
            Quantity = 21,
            UnitPrice = 10.00m
        };

        var exception = Assert.Throws<InvalidOperationException>(() => saleItem.ApplyDiscount());
        Assert.Equal("Cannot sell more than 20 identical items", exception.Message);
    }

    /// <summary>
    /// Tests that validation passes when all sale item properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale item data")]
    public void Given_ValidSaleItemData_When_Validated_Then_ShouldReturnValid()
    {
        var saleItem = SaleTestData.GenerateValidSaleItem();

        var result = saleItem.Validate();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale item properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale item data")]
    public void Given_InvalidSaleItemData_When_Validated_Then_ShouldReturnInvalid()
    {
        var saleItem = new SaleItem
        {
            ProductName = SaleTestData.GenerateInvalidProductName(),
            Quantity = SaleTestData.GenerateInvalidQuantity(),
            UnitPrice = SaleTestData.GenerateInvalidUnitPrice(),
            Discount = SaleTestData.GenerateInvalidDiscount()
        };

        var result = saleItem.Validate();

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    /// <summary>
    /// Tests that the sale item constructor initializes CreatedAt to current UTC time.
    /// </summary>
    [Fact(DisplayName = "Sale item constructor should initialize CreatedAt to current UTC time")]
    public void Given_NewSaleItem_When_Created_Then_CreatedAtShouldBeSet()
    {
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        var saleItem = new SaleItem();

        Assert.True(saleItem.CreatedAt >= beforeCreation);
        Assert.True(saleItem.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    /// <summary>
    /// Tests that CalculateTotalAmount updates the UpdatedAt timestamp.
    /// </summary>
    [Fact(DisplayName = "CalculateTotalAmount should update the UpdatedAt timestamp")]
    public void Given_SaleItem_When_CalculateTotalAmount_Then_UpdatedAtShouldBeSet()
    {
        var saleItem = SaleTestData.GenerateValidSaleItem();
        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        saleItem.CalculateTotalAmount();

        Assert.True(saleItem.UpdatedAt >= beforeUpdate);
        Assert.True(saleItem.UpdatedAt <= DateTime.UtcNow.AddSeconds(1));
    }

    /// <summary>
    /// Tests that ApplyDiscount updates the UpdatedAt timestamp.
    /// </summary>
    [Fact(DisplayName = "ApplyDiscount should update the UpdatedAt timestamp")]
    public void Given_SaleItem_When_ApplyDiscount_Then_UpdatedAtShouldBeSet()
    {
        var saleItem = new SaleItem
        {
            Quantity = 5,
            UnitPrice = 10.00m
        };
        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        saleItem.ApplyDiscount();

        Assert.True(saleItem.UpdatedAt >= beforeUpdate);
        Assert.True(saleItem.UpdatedAt <= DateTime.UtcNow.AddSeconds(1));
    }
}