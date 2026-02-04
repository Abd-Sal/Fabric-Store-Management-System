namespace FabricesStoreManagementSystem.Tests.Validations;

public class CatalogProductValidationsTests
{
    private readonly CatalogProductValidations _validator = new();

    private static CatalogProductRequest CreateValidRequest() => new(
        ProductID: Guid.NewGuid(),
        Quantity: 10.5f
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
        var request = CreateValidRequest() with { Quantity = 0f };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية مطلوبة.");
    }

    [Fact]
    public void Quantity_WhenNegative_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = -5f };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يجب أن تكون أكبر من الصفر.");
    }

    [Fact]
    public void Quantity_WhenTooSmall_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 0.05f }; // < 0.1

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يجب أن تكون 0.1 على الأقل.");
    }

    [Fact]
    public void Quantity_WhenTooLarge_ShouldHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 150f }; // > 100

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية لا يمكن أن تتجاوز 100.");
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void Quantity_WhenInvalidFloat_ShouldHaveValidationError(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        var result = _validator.TestValidate(request);

        // Should fail validation (don't check specific message due to cascade)
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(1.25f)]   // Two decimal places
    [InlineData(10.123f)] // Three decimal places
    [InlineData(0.75f)]   // Two decimal places
    public void Quantity_WhenMoreThanOneDecimalPlace_ShouldHaveValidationError(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى.");
    }

    [Theory]
    [InlineData(1f)]      // No decimal places
    [InlineData(10.5f)]   // One decimal place
    [InlineData(0.1f)]    // One decimal place (minimum)
    [InlineData(100f)]    // No decimal places (maximum)
    [InlineData(50.0f)]   // One decimal place (zero)
    public void Quantity_WhenValid_ShouldNotHaveValidationError(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

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
            new CatalogProductRequest(Guid.Empty, 10.5f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 0f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), -5f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 0.05f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 150f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), float.NaN)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 1.25f)
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
            new CatalogProductRequest(Guid.NewGuid(), 0.1f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 1f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 10.5f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 50.0f)
        };

        yield return new object[]
        {
            new CatalogProductRequest(Guid.NewGuid(), 100f)
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForProductID()
    {
        var request = new CatalogProductRequest(Guid.Empty, -5f); // Both invalid

        var result = _validator.TestValidate(request);

        // Should have error for ProductID first
        result.ShouldHaveValidationErrorFor(x => x.ProductID);
    }

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForQuantity()
    {
        var request = new CatalogProductRequest(Guid.NewGuid(), 0f);

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية مطلوبة.");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Quantity_WhenExactlyMinValue_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 0.1f };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Quantity_WhenExactlyMaxValue_ShouldNotHaveValidationError()
    {
        var request = CreateValidRequest() with { Quantity = 100f };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0.1f)]
    [InlineData(1.0f)]
    [InlineData(99.9f)]
    [InlineData(100.0f)]
    public void Quantity_WhenWithinRange_ShouldNotHaveValidationError(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(0.099999f)]    // Just below minimum
    [InlineData(100.0001f)]    // Just above maximum
    public void Quantity_WhenOutsideRange_ShouldHaveValidationError(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    #endregion

    #region Decimal Place Validation Tests

    [Theory]
    [InlineData(10f)]      // No decimal: 10
    [InlineData(10.0f)]    // One decimal (zero): 10.0
    [InlineData(10.5f)]    // One decimal: 10.5
    [InlineData(0.1f)]     // One decimal: 0.1
    [InlineData(99.9f)]    // One decimal: 99.9
    public void Quantity_WhenZeroOrOneDecimalPlace_ShouldPassValidation(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Theory]
    [InlineData(10.55f)]   // Two decimals: 10.55
    [InlineData(0.12f)]    // Two decimals: 0.12
    [InlineData(99.99f)]   // Two decimals: 99.99
    [InlineData(10.123f)]  // Three decimals: 10.123
    public void Quantity_WhenMoreThanOneDecimalPlace_ShouldFailValidation(float quantity)
    {
        var request = CreateValidRequest() with { Quantity = quantity };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى.");
    }

    #endregion
}