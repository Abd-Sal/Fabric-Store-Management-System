namespace FabricesStoreManagementSystem.Tests.Validations;

public class PurchaseItemValidationsTests
{
    private readonly PurchaseItemValidations _validator = new();

    private static PurchaseItemRequest CreateValidRequest(
        Guid? productId = null,
        decimal quantity = 1.0m,
        decimal unitCost = 10.00m)
    {
        return new PurchaseItemRequest(
            productId ?? Guid.NewGuid(),
            quantity,
            unitCost
        );
    }
    [Fact]
    public void ProductID_Empty_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(productId: Guid.Empty);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductID);
    }

    [Fact]
    public void ProductID_Valid_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.ProductID);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UnitCost_LessThanOrEqualZero_ShouldHaveValidationError(decimal unitCost)
    {
        var model = CreateValidRequest(unitCost: unitCost);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.UnitCost);
    }

    [Theory]
    [InlineData(1.0011)]
    [InlineData(10.9991)]
    public void UnitCost_MoreThanTwoDecimalPlaces_ShouldHaveValidationError(decimal unitCost)
    {
        var model = CreateValidRequest(unitCost: unitCost);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.UnitCost);
    }

    [Theory]
    [InlineData(1.00)]
    [InlineData(1.50)]
    [InlineData(999999.99)]
    public void UnitCost_Valid_ShouldNotHaveValidationError(decimal unitCost)
    {
        var model = CreateValidRequest(unitCost: unitCost);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.UnitCost);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Quantity_LessThanOrEqualZero_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.11)]
    [InlineData(2.25)]
    [InlineData(9.99)]
    public void Quantity_MoreThanOneDecimalPlace_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.15)]
    [InlineData(2.35)]
    [InlineData(7.77)]
    public void Quantity_NotMultipleOfPointOne_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    [InlineData(9.9)]
    [InlineData(10.0)]
    public void Quantity_Valid_ShouldNotHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }
}
