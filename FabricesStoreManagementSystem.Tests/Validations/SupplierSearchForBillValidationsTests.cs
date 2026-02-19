namespace FabricesStoreManagementSystem.Tests.Validations;

public class SupplierSearchForBillValidationsTests
{
    private readonly SupplierSearchForBillValidations _validator = new();

    [Fact]
    public void Validate_WithValidName_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد أحمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidEnglishName_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "Mohamed Ahmed");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidMixedName_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد Ahmed-123_456");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingHyphen_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "عبد-الرحمن");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingUnderscore_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد_أحمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullName_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم لا يمكن أن يكون فارغاً");
    }

    [Fact]
    public void Validate_WithEmptyStringName_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم مطلوب");
    }

    [Fact]
    public void Validate_WithWhitespaceName_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "   ");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم لا يمكن أن يكون مسافات فقط");
    }

    [Fact]
    public void Validate_WithSingleCharacterName_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "أ");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يكون على الأقل حرفين");
    }

    [Fact]
    public void Validate_WithSingleCharacterWithSpacesName_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: " أ ");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يكون على الأقل حرفين");
    }

    [Fact]
    public void Validate_WithNameExceedingMaxLength_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: new string('أ', 51));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن لا يتجاوز 50 حرف");
    }

    [Fact]
    public void Validate_WithNameAtMaxLength_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: new string('أ', 50));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNameContainingSpecialCharacters_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد@أحمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingInvalidSymbols_ShouldFail()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد#أحمد$123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingNumbers_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد 123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNameContainingOnlyValidCharacters_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "ABC_123-def");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNameContainingMultipleSpaces_ShouldPass()
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: "محمد   أحمد   علي");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("مح")]
    [InlineData("a1")]
    [InlineData("أ-")]
    [InlineData("أ_")]
    public void Validate_WithMinimumLengthValidNames_ShouldPass(string name)
    {
        // Arrange
        var request = new SupplierSearchForBillRequest(Name: name);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ShouldStopAtFirstError()
    {
        // This test verifies the CascadeMode.Stop behavior
        // Since Name is null, it should only show the NotNull error and stop

        // Arrange
        var request = new SupplierSearchForBillRequest(Name: null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.Errors.Should().HaveCount(1);
        result.Errors.First().ErrorMessage.Should().Be("الاسم لا يمكن أن يكون فارغاً");
    }
}