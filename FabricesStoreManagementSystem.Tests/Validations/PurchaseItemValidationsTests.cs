namespace FabricesStoreManagementSystem.Tests.Validations;

public class PurchaseItemValidationsTests
{
    private readonly PurchaseItemValidations _validator = new();

    private static PurchaseItemRequest CreateValidRequest(
        Guid? productId = null,
        float quantity = 1.0f,
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
    [InlineData(1.001)]
    [InlineData(10.999)]
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
    [InlineData(0f)]
    [InlineData(-1f)]
    public void Quantity_LessThanOrEqualZero_ShouldHaveValidationError(float quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Quantity_InvalidFloat_ShouldHaveValidationError(float quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.11f)]
    [InlineData(2.25f)]
    [InlineData(9.99f)]
    public void Quantity_MoreThanOneDecimalPlace_ShouldHaveValidationError(float quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.15f)]
    [InlineData(2.35f)]
    [InlineData(7.77f)]
    public void Quantity_NotMultipleOfPointOne_ShouldHaveValidationError(float quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.0f)]
    [InlineData(1.5f)]
    [InlineData(2.0f)]
    [InlineData(9.9f)]
    [InlineData(10.0f)]
    public void Quantity_Valid_ShouldNotHaveValidationError(float quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }
}
