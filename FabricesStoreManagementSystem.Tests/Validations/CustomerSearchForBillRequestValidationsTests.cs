namespace FabricesStoreManagementSystem.Tests.Validations;

public class CustomerSearchForBillRequestValidationsTests
{
    private readonly CustomerSearchForBillValidations _validator = new();

    [Fact]
    public void Validate_WithValidArabicName_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidEnglishName_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "Ahmed Mohamed");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidMixedName_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد Ahmed-123_456");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingHyphen_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "عبد-الرحمن");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingUnderscore_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد_محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingNumbers_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد 123");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameContainingMultipleSpaces_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد   محمد   علي");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithValidNameAtMaxLength_ShouldPass()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: new string('أ', 50));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullName_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: null!);

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
        var request = new CustomerSearchForBillRequest(Name: "");

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
        var request = new CustomerSearchForBillRequest(Name: "   ");

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
        var request = new CustomerSearchForBillRequest(Name: "أ");

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
        var request = new CustomerSearchForBillRequest(Name: " أ ");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يكون على الأقل حرفين");
    }

    [Fact]
    public void Validate_WithSingleEnglishCharacterName_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "A");

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
        var request = new CustomerSearchForBillRequest(Name: new string('أ', 51));

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن لا يتجاوز 50 حرف");
    }

    [Fact]
    public void Validate_WithNameContainingAtSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد@محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingHashSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد#محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingDollarSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد$محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingPercentSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد%محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingAsterisk_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد*محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingPlusSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد+محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingEqualsSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد=محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingSlash_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد/محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingBackslash_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد\\محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingPipeSymbol_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد|محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingQuestionMark_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد?محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithNameContainingExclamationMark_ShouldFail()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: "أحمد!محمد");

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Theory]
    [InlineData("ab")]
    [InlineData("مح")]
    [InlineData("a1")]
    [InlineData("أ-")]
    [InlineData("أ_")]
    [InlineData("A B")]
    [InlineData("أ ب")]
    [InlineData("123")]
    [InlineData("ABC")]
    public void Validate_WithMinimumLengthValidNames_ShouldPass(string name)
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: name);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("أحمد@محمد")]
    [InlineData("أحمد#محمد")]
    [InlineData("أحمد$محمد")]
    [InlineData("أحمد%محمد")]
    [InlineData("أحمد^محمد")]
    [InlineData("أحمد&محمد")]
    [InlineData("أحمد*محمد")]
    [InlineData("أحمد(محمد")]
    [InlineData("أحمد)محمد")]
    [InlineData("أحمد+محمد")]
    [InlineData("أحمد=محمد")]
    [InlineData("أحمد{محمد")]
    [InlineData("أحمد}محمد")]
    [InlineData("أحمد[محمد")]
    [InlineData("أحمد]محمد")]
    [InlineData("أحمد;محمد")]
    [InlineData("أحمد:محمد")]
    [InlineData("أحمد'محمد")]
    [InlineData("أحمد\"محمد")]
    [InlineData("أحمد<محمد")]
    [InlineData("أحمد>محمد")]
    [InlineData("أحمد,محمد")]
    [InlineData("أحمد.محمد")]
    [InlineData("أحمد/محمد")]
    [InlineData("أحمد\\محمد")]
    [InlineData("أحمد|محمد")]
    [InlineData("أحمد`محمد")]
    [InlineData("أحمد~محمد")]
    [InlineData("أحمد!محمد")]
    [InlineData("أحمد?محمد")]
    public void Validate_WithInvalidSpecialCharacters_ShouldFail(string name)
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: name);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }

    [Fact]
    public void Validate_WithMultipleValidationErrors_ShouldStopAtFirstError()
    {
        // This test verifies the CascadeMode.Stop behavior
        // Since Name is null, it should only show the NotNull error and stop

        // Arrange
        var request = new CustomerSearchForBillRequest(Name: null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.Errors.Should().HaveCount(1);
        result.Errors.First().ErrorMessage.Should().Be("الاسم لا يمكن أن يكون فارغاً");
    }

    [Fact]
    public void Validate_WithNullName_ShouldNotShowOtherValidationErrors()
    {
        // Arrange
        var request = new CustomerSearchForBillRequest(Name: null!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.Errors.Should().HaveCount(1);
        // Verify that other error messages are not present
        result.Errors.Should().NotContain(e => e.ErrorMessage == "الاسم مطلوب");
        result.Errors.Should().NotContain(e => e.ErrorMessage == "الاسم لا يمكن أن يكون مسافات فقط");
        result.Errors.Should().NotContain(e => e.ErrorMessage == "الاسم يجب أن يكون على الأقل حرفين");
    }
}