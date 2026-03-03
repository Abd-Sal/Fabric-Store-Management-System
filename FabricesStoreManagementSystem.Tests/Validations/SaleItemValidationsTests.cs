namespace FabricesStoreManagementSystem.Tests.Validations;

public class SaleItemValidationsTests
{
    private readonly SaleItemValidations _validator = new();

    private static SaleItemRequest CreateValidRequest(
        Guid? productId = null,
        decimal quantity = 1.0m,
        decimal unitPrice = 10.00m)
    {
        return new SaleItemRequest(
            ProductID: productId ?? Guid.NewGuid(),
            Quantity: quantity,
            UnitPrice: unitPrice
        );
    }

    #region ProductID

    [Fact]
    public void ProductID_WhenEmptyGuid_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(productId: Guid.Empty);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProductID);
    }

    [Fact]
    public void ProductID_WithValidGuid_ShouldNotHaveValidationError()
    {
        var model = CreateValidRequest();

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.ProductID);
    }

    #endregion

    #region Quantity - Required / Range

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    public void Quantity_LessThanOrEqualZero_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Quantity_ExceedingMax_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(quantity: 10000.1m);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Quantity - Precision

    [Theory]
    [InlineData(1.11)]
    [InlineData(2.555)]
    [InlineData(0.123)]
    public void Quantity_WithMoreThanOneDecimalPlace_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(10.1)]
    public void Quantity_WithOneDecimalPlace_ShouldNotHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Quantity - Increment (0.1)

    [Theory]
    [InlineData(1.05)]
    [InlineData(0.25)]
    [InlineData(2.15)]
    public void Quantity_NotMultipleOfPointOne_ShouldHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(2.0)]
    [InlineData(9.9)]
    public void Quantity_MultipleOfPointOne_ShouldNotHaveValidationError(decimal quantity)
    {
        var model = CreateValidRequest(quantity: quantity);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region UnitPrice - Required / Range

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void UnitPrice_LessThanOrEqualZero_ShouldHaveValidationError(decimal price)
    {
        var model = CreateValidRequest(unitPrice: price);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    [Fact]
    public void UnitPrice_ExceedingMax_ShouldHaveValidationError()
    {
        var model = CreateValidRequest(unitPrice: 1_000_000.01m);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    #endregion

    #region UnitPrice - Precision

    [Theory]
    [InlineData(10.001)]
    [InlineData(5.999)]
    public void UnitPrice_WithMoreThanTwoDecimalPlaces_ShouldHaveValidationError(decimal price)
    {
        var model = CreateValidRequest(unitPrice: price);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(10.5)]
    [InlineData(10.55)]
    public void UnitPrice_WithValidPrecision_ShouldNotHaveValidationError(decimal price)
    {
        var model = CreateValidRequest(unitPrice: price);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
    }

    #endregion

    #region Happy Path

    [Fact]
    public void SaleItemRequest_WithValidValues_ShouldPassValidation()
    {
        var model = CreateValidRequest(
            quantity: 2.5m,
            unitPrice: 99.99m
        );

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
