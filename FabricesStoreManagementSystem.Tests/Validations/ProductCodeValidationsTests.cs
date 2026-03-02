namespace FabricesStoreManagementSystem.Tests.Validations;

public class ProductCodeValidationsTests
{
    private readonly ProductCodeValidations _validator = new();

    [Fact]
    public void Should_Have_Error_When_Code_Is_Empty()
    {
        // Arrange
        var request = new ProductCodeRequest(string.Empty);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج مطلوب");
    }

    [Fact]
    public void Should_Have_Error_When_Code_Is_Null()
    {
        // Arrange
        var request = new ProductCodeRequest(null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج مطلوب");
    }

    [Fact]
    public void Should_Have_Error_When_Code_Is_Whitespace()
    {
        // Arrange
        var request = new ProductCodeRequest("   ");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج مطلوب");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("_")]
    [InlineData("-")]
    public void Should_Not_Have_Error_When_Code_Has_Minimum_Length(string validCode)
    {
        // Arrange
        var request = new ProductCodeRequest(validCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Has_Exactly_Maximum_Length()
    {
        // Arrange
        var maxLengthCode = new string('A', ProductConfigurations.CodeMaxLength);
        var request = new ProductCodeRequest(maxLengthCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Have_Error_When_Code_Exceeds_Maximum_Length()
    {
        // Arrange
        var tooLongCode = new string('A', ProductConfigurations.CodeMaxLength + 1);
        var request = new ProductCodeRequest(tooLongCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage($"يجب أن يكون رمز المنتج على الأكثر {ProductConfigurations.CodeMaxLength} حرف");
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("product-123")]
    [InlineData("code_123")]
    [InlineData("TEST")]
    [InlineData("123")]
    [InlineData("A")]
    public void Should_Not_Have_Error_For_Valid_Codes(string validCode)
    {
        // Arrange
        var request = new ProductCodeRequest(validCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Code_Is_Invalid_In_Multiple_Ways()
    {
        // Arrange
        var request = new ProductCodeRequest(null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code);
        var errors = result.Errors.Where(e => e.PropertyName == "code");
        Assert.Contains(errors, e => e.ErrorMessage == "رمز المنتج مطلوب");
    }

    [Fact]
    public void Should_Have_Exactly_Two_Error_Messages_For_Null_Code()
    {
        // Arrange
        var request = new ProductCodeRequest(null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        var errors = result.Errors.Where(e => e.PropertyName == "code").ToList();
        Assert.Equal(2, errors.Count); // NotNull and NotEmpty
        Assert.Contains(errors, e => e.ErrorMessage == "رمز المنتج مطلوب");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(ProductConfigurations.CodeMaxLength)]
    public void Should_Validate_Codes_With_Different_Lengths(int length)
    {
        // Arrange
        var code = new string('A', length);
        var request = new ProductCodeRequest(code);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        if (length <= ProductConfigurations.CodeMaxLength)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.code);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.code);
        }
    }
}