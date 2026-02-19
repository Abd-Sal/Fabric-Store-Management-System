namespace FabricesStoreManagementSystem.Tests.Validations;

public class SearchProductBillByCodeValidationsTests
{
    private readonly SearchProductBillByCodeValidations _validator = new();

    [Fact]
    public void Should_Have_Error_When_Code_Is_Empty()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("");

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
        var request = new SearchProductBillByCodeRequest(null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج لا يمكن أن يكون فارغاً");
    }

    [Fact]
    public void Should_Have_Error_When_Code_Is_Less_Than_Minimum_Length()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("A");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج يجب أن يكون على الأقل حرفين");
    }

    [Fact]
    public void Should_Have_Error_When_Code_Exceeds_Maximum_Length()
    {
        // Arrange
        var longCode = new string('A', 51);
        var request = new SearchProductBillByCodeRequest(longCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج يجب أن لا يتجاوز 50 حرف");
    }

    [Fact]
    public void Should_Have_Error_When_Code_Contains_Invalid_Characters()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("ABC@123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code)
            .WithErrorMessage("رمز المنتج يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Only_English_Letters_And_Numbers()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("ABC123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_English_Letters_Numbers_And_Dash()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("ABC-123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_English_Letters_Numbers_And_Underscore()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("ABC_123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Only_Arabic_Letters()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("منتج");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Arabic_Letters_And_Numbers()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("منتج123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Arabic_Letters_Numbers_And_Dash()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("منتج-123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Mix_Of_Arabic_English_And_Numbers()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("منتجABC123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Contains_Spaces()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("منتج ABC 123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Is_Exactly_Minimum_Length()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("AB");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Code_Is_Exactly_Maximum_Length()
    {
        // Arrange
        var exactLengthCode = new string('A', 50);
        var request = new SearchProductBillByCodeRequest(exactLengthCode);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.code);
    }

    [Fact]
    public void Should_Have_Multiple_Errors_When_Code_Is_Empty_And_Invalid()
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest("");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.code);
        var errors = result.Errors.Select(e => e.ErrorMessage);
        Assert.Contains("رمز المنتج مطلوب", errors);
    }

    [Theory]
    [InlineData("ABC-123", true)]
    [InlineData("منتج-123", true)]
    [InlineData("ABC_123", true)]
    [InlineData("منتج_123", true)]
    [InlineData("ABC 123", true)]
    [InlineData("منتج 123", true)]
    [InlineData("A", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("ABC@123", false)]
    [InlineData("ABC#123", false)]
    [InlineData("ABC$123", false)]
    [InlineData("ABC%123", false)]
    [InlineData("ABC^123", false)]
    [InlineData("ABC&123", false)]
    [InlineData("ABC*123", false)]
    [InlineData("ABC(123", false)]
    [InlineData("ABC)123", false)]
    [InlineData("ABC+123", false)]
    [InlineData("ABC=123", false)]
    [InlineData("ABC{123", false)]
    [InlineData("ABC}123", false)]
    [InlineData("ABC[123", false)]
    [InlineData("ABC]123", false)]
    [InlineData("ABC|123", false)]
    [InlineData("ABC\\123", false)]
    [InlineData("ABC:123", false)]
    [InlineData("ABC;123", false)]
    [InlineData("ABC'123", false)]
    [InlineData("ABC\"123", false)]
    [InlineData("ABC<123", false)]
    [InlineData("ABC>123", false)]
    [InlineData("ABC,123", false)]
    [InlineData("ABC.123", false)]
    [InlineData("ABC?123", false)]
    [InlineData("ABC/123", false)]
    public void Test_Code_Validation(string? code, bool shouldBeValid)
    {
        // Arrange
        var request = new SearchProductBillByCodeRequest(code!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        if (shouldBeValid)
        {
            result.ShouldNotHaveValidationErrorFor(x => x.code);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(x => x.code);
        }
    }
}