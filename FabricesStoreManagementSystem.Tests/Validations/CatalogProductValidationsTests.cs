using System.Globalization;

namespace FabricesStoreManagementSystem.Tests.Validations;

public class CatalogProductValidationsTests
{
    private readonly CatalogProductValidations _validator = new();
    private static decimal D(string value) => decimal.Parse(value, CultureInfo.InvariantCulture);

    private static CatalogProductRequest CreateValidRequest() => new(
        ProductID: Guid.NewGuid(),
        Quantity: 10.5m
    );

    #region ProductID Validation Tests

    [Fact]
    public void ProductID_WhenEmptyGuid_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { ProductID = Guid.Empty };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.ProductID)
            .WithErrorMessage("معرف المنتج مطلوب.");
    }

    [Fact]
    public void ProductID_WhenValid_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.ProductID);
    }

    #endregion

    #region Quantity Validation Tests

    [Fact]
    public void Quantity_WhenZero_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 0m };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية مطلوبة.");
    }

    [Fact]
    public void Quantity_WhenNegative_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = -5m };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يجب أن تكون أكبر من الصفر.");
    }

    [Fact]
    public void Quantity_WhenTooSmall_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 0.05m }; // < 0.1

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يجب أن تكون 0.1 على الأقل.");
    }

    [Fact]
    public void Quantity_WhenTooLarge_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 150m }; // > 100

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية لا يمكن أن تتجاوز 100.");
    }

    [Theory]
    [InlineData("1.251")]   // Two decimal places
    [InlineData("10.1213")] // Three decimal places
    [InlineData("0.751")]   // Two decimal places
    public void Quantity_WhenMoreThanOneDecimalPlace_ShouldHaveValidationError(string quantity)
    {
        var request = CreateValidRequest() with { Quantity = D(quantity) };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.");
    }

    [Theory]
    [InlineData("1")]      // No decimal places
    [InlineData("10.5")]   // One decimal place
    [InlineData("0.1")]    // One decimal place (minimum)
    [InlineData("100")]    // No decimal places (maximum)
    [InlineData("50.0")]   // One decimal place (zero)
    public void Quantity_WhenValid_ShouldNotHaveValidationError(string quantity)
    {
        var request = CreateValidRequest() with { Quantity = D(quantity) };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Complete Request Validation Tests

    [Fact]
    public void Request_WhenAllFieldsValid_ShouldPassAllValidations()
    {
        var request = CreateValidRequest();

        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(GetInvalidRequestTestData))]
    public void Request_WhenInvalid_ShouldHaveValidationErrors(CatalogProductRequest request)
    {
        var result = _validator.TestValidate(request);

        Assert.NotEmpty(result.Errors);
    }

    public static IEnumerable<object[]> GetInvalidRequestTestData()
    {
        yield return new object[]
        {
            new CatalogProductRequest(Guid.Empty, 10.5m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 0m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), -5m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 0.05m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 150m)
        };
    }

    [Theory]
    [MemberData(nameof(GetValidRequestTestData))]
    public void Request_WhenValid_ShouldNotHaveValidationErrors(CatalogProductRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidRequestTestData()
    {
        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 0.1m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 1m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 10.5m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 50.0m)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 100m)
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForProductID()
    {
        var request = new CatalogProductRequest(Guid.Empty, -5m); // Both invalid

        var result = _validator.TestValidate(request);

        // Should have error for ProductID first
        result.ShouldHaveValidationErrorFor(x => x.ProductID);
    }

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForQuantity()
    {
        var request = new CatalogProductRequest(Guid.NewGuid(), 0m);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية مطلوبة.");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Quantity_WhenExactlyMinValue_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 0.1m };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Quantity_WhenExactlyMaxValue_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 100m };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(1.0)]
    [InlineData(99.9)]
    [InlineData(100.0)]
    public void Quantity_WhenWithinRange_ShouldNotHaveValidationError(decimal quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0.099999)]    // Just below minimum
    [InlineData(100.0001)]    // Just above maximum
    public void Quantity_WhenOutsideRange_ShouldHaveValidationError(decimal quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Decimal Place Validation Tests

    [Theory]
    [InlineData(10.0)]      // No decimal: 10
    [InlineData(10.0)]    // One decimal (zero): 10.0
    [InlineData(10.5)]    // One decimal: 10.5
    [InlineData(0.1)]     // One decimal: 0.1
    [InlineData(99.9)]    // One decimal: 99.9
    public void Quantity_WhenZeroOrOneDecimalPlace_ShouldPassValidation(decimal quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(10.551)]   // Two decimals: 10.55
    [InlineData(0.121)]    // Two decimals: 0.12
    [InlineData(99.991)]   // Two decimals: 99.99
    [InlineData(10.1231)]  // Three decimals: 10.123
    public void Quantity_WhenMoreThanOneDecimalPlace_ShouldFailValidation(decimal quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.");
    }

    #endregion
}