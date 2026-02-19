namespace FabricesStoreManagementSystem.Tests.Validations;

public class CustomerValidationsTests
{
    private readonly CustomerValidations _validator = new();

    #region FirstName Validation Tests

    [Theory]
    [InlineData(null, "الاسم الأول مطلوب.")]
    [InlineData("", "الاسم الأول مطلوب.")]
    [InlineData("   ", "الاسم الأول مطلوب.")]
    public void FirstName_WhenNullOrWhiteSpace_ShouldHaveValidationError(string? firstName, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { FirstName = firstName! };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("أ", "الاسم الأول يجب أن يحتوي على حرفين على الأقل.")]
    [InlineData("a", "الاسم الأول يجب أن يحتوي على حرفين على الأقل.")]
    public void FirstName_WhenTooShort_ShouldHaveValidationError(string? firstName, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { FirstName = firstName! };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("محمد123", "الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("Ahmed@", "الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("خالد#", "الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("John$Doe", "الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    public void FirstName_WhenContainsInvalidCharacters_ShouldHaveValidationError(string? firstName, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { FirstName = firstName! };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("محمد")]
    [InlineData("أحمد بن علي")]
    [InlineData("John")]
    [InlineData("Mary-Jane")]
    [InlineData("O'Connor")]
    [InlineData("محمد أحمد")]
    [InlineData("Khaled Al-Mansour")]
    [InlineData("عبد الرحمن")]
    public void FirstName_WhenValid_ShouldNotHaveValidationError(string firstName)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { FirstName = firstName };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.FirstName);
    }

    #endregion

    #region LastName Validation Tests

    [Theory]
    [InlineData(null, "اسم العائلة مطلوب.")]
    [InlineData("", "اسم العائلة مطلوب.")]
    [InlineData("   ", "اسم العائلة مطلوب.")]
    public void LastName_WhenNullOrWhiteSpace_ShouldHaveValidationError(string? lastName, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { LastName = lastName! };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("الخالد123", "اسم العائلة يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("Al-Saud@", "اسم العائلة يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("بن#", "اسم العائلة يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    [InlineData("Smith&Jones", "اسم العائلة يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")]
    public void LastName_WhenContainsInvalidCharacters_ShouldHaveValidationError(string? lastName, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { LastName = lastName! };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("الخالد")]
    [InlineData("بن علي")]
    [InlineData("العتيبي")]
    [InlineData("Al-Mansour")]
    [InlineData("O'Reilly")]
    [InlineData("الراشد")]
    [InlineData("ابن سينا")]
    [InlineData("Smith")]
    [InlineData("Johnson")]
    [InlineData("Al-Ghamdi")]
    [InlineData("O'Brien")]
    public void LastName_WhenValid_ShouldNotHaveValidationError(string lastName)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { LastName = lastName };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.LastName);
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("not-an-email", "صيغة البريد الإلكتروني غير صالحة.")]
    [InlineData("@no-username.com", "صيغة البريد الإلكتروني غير صالحة.")] // Changed from "صحيحة" to "صالحة"
    [InlineData("no-domain@", "صيغة البريد الإلكتروني غير صالحة.")] // Changed from "صحيحة" to "صالحة"
    [InlineData("spaces in@email.com", "صيغة البريد الإلكتروني غير صحيحة.")] // This will fail the Regex first
    [InlineData("invalid@domain", "صيغة البريد الإلكتروني غير صحيحة.")] // This will fail the Regex first
    [InlineData("test@.com", "صيغة البريد الإلكتروني غير صحيحة.")] // This will fail the Regex first
    public void Email_WhenProvidedAndInvalid_ShouldHaveValidationError(string email, string expectedError)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { Email = email, Phone = null };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(expectedError);
    }

    // Or better yet, let's test which validator triggers which error:

    [Theory]
    [InlineData("@no-username.com")] // Will trigger EmailAddress validator
    [InlineData("no-domain@")] // Will trigger EmailAddress validator
    public void Email_WhenInvalidFormat_ShouldHaveEmailAddressErrorMessage(string email)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { Email = email, Phone = null };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("صيغة البريد الإلكتروني غير صالحة.");
    }

    [Theory]
    [InlineData("spaces in@email.com")] // Contains spaces
    [InlineData("invalid@domain")] // Missing TLD
    [InlineData("test@.com")] // Missing username
    public void Email_WhenInvalidRegexPattern_ShouldHaveRegexErrorMessage(string email)
    {
        // Arrange
        var request = CreateValidCustomerRequest() with { Email = email, Phone = null };

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("صيغة البريد الإلكتروني غير صحيحة.");
    }

    #endregion

    #region Phone Validation Tests

    [Theory]
    [InlineData("invalid-phone")]
    [InlineData("+++()()()")]
    [InlineData("123")]
    [InlineData("12 34")]
    [InlineData("1234567890123456712345678901234567")]
    public void Phone_WhenInvalid_ShouldHaveValidationError(string phone)
    {
        var request = CreateValidCustomerRequest() with
        {
            Phone = phone,
            Email = "test@example.com"
        };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Phone_WhenWhitespace_ShouldTriggerBusinessRule(string phone)
    {
        var request = new CustomerRequest(
            "محمد",
            "الخالد",
            null,
            phone,
            "عنوان"
        );

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor("ContactInformation")
            .WithErrorMessage("يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).");
    }

    [Theory]
    [InlineData("   ")]
    [InlineData(" ")]
    public void Phone_WhenWhitespaceWithEmail_ShouldNotHaveErrors(string phone)
    {
        var request = new CustomerRequest(
            "محمد",
            "الخالد",
            "test@example.com",
            phone,
            "عنوان"
        );

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        result.ShouldNotHaveValidationErrorFor("ContactInformation");
    }

    [Fact]
    public void Phone_WhenTooLong_ShouldHaveLengthError()
    {
        var longPhone = new string('1', CustomerConfigurations.PhoneMaxLength + 1);
        var request = CreateValidCustomerRequest() with { Phone = longPhone };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage($"رقم الهاتف يجب أن يكون بين 1 و {CustomerConfigurations.PhoneMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    [InlineData("+12345678900")]
    [InlineData("0123456789")]
    [InlineData("966112345678")]
    [InlineData("00966112345678")]
    [InlineData("+966551234567")]
    public void Phone_WhenValid_ShouldNotHaveValidationError(string phone)
    {
        var request = new CustomerRequest(
            "محمد",
            "الخالد",
            null,
            phone,
            "عنوان"
        );

        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Phone_WhenNullOrEmpty_ShouldNotHaveValidationError(string? phone)
    {
        var request = CreateValidCustomerRequest() with { Phone = phone! };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    #endregion

    #region Address Validation Tests

    [Theory]
    [InlineData("   ")]
    [InlineData(" ")]
    [InlineData("\t")]
    public void Address_WhenWhitespace_ShouldNotTriggerValidation(string address)
    {
        var request = CreateValidCustomerRequest() with { Address = address };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Address_WhenTooLong_ShouldHaveValidationError()
    {
        var longAddress = new string('أ', CustomerConfigurations.AddressMaxLength + 1);
        var request = CreateValidCustomerRequest() with { Address = longAddress };

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage($"العنوان يجب أن يكون بين 1 و {CustomerConfigurations.AddressMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("عنوان é")] // é is Extended Latin, not Basic Latin
    [InlineData("شارع Дворцовая")] // Д is Cyrillic
    [InlineData("東京住所")] // Japanese characters
    [InlineData("عنوان ≠ عنوان")] // ≠ is mathematical symbol
    [InlineData("€ 100 Street")] // € is currency symbol
    [InlineData("Copyright© Street")] // © is copyright symbol
    public void Address_WhenContainsInvalidCharacters_ShouldHaveValidationError(string address)
    {
        var request = CreateValidCustomerRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("العنوان يحتوي على أحرف غير مسموح بها.");
    }

    // Test that allowed characters don't trigger errors
    [Theory]
    [InlineData("123 Main St.")] // Period is allowed
    [InlineData("Street, Avenue")] // Comma is allowed
    [InlineData("العنوان-الرئيسي")] // Dash is allowed
    [InlineData("P.O. Box 123")] // Period is allowed
    [InlineData("user@example.com")] // @ is in BasicLatin, so allowed!
    public void Address_WhenContainsAllowedCharacters_ShouldNotHaveValidationError(string address)
    {
        var request = CreateValidCustomerRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Address_WhenNotProvided_ShouldNotHaveValidationError(string? address)
    {
        var request = CreateValidCustomerRequest() with { Address = address! };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData("شارع الملك فهد، الرياض 12345")]
    [InlineData("123 Main Street, New York, NY 10001")]
    [InlineData("حي المروج، جدة 23456")]
    [InlineData("P.O. Box 1234, Dubai 54321")]
    [InlineData("الطابق 3، شقة 12، مبنى النخيل")]
    public void Address_WhenValid_ShouldNotHaveValidationError(string address)
    {
        var request = CreateValidCustomerRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    #endregion

    #region Contact Information Business Rule Tests

    [Theory]
    [InlineData(null, null, "يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")]
    [InlineData("", "", "يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")]
    [InlineData("   ", "   ", "يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")]
    [InlineData(null, "", "يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")]
    [InlineData("", null, "يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")]
    public void Customer_WhenNoContactInformationProvided_ShouldHaveValidationError(string? email, string? phone, string expectedError)
    {
        // Arrange
        var request = new CustomerRequest(
            "محمد",
            "الخالد",
            email!,
            phone!,
            "عنوان"
        );

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor("ContactInformation")
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("test@example.com", null)]
    [InlineData(null, "0123456789")]
    [InlineData("test@example.com", "0123456789")]
    [InlineData("test@example.com", "")]
    [InlineData("", "0123456789")]
    public void Customer_WhenAtLeastOneContactProvided_ShouldNotHaveValidationError(string? email, string? phone)
    {
        // Arrange
        var request = new CustomerRequest(
            "محمد",
            "الخالد",
            email!,
            phone!,
            "عنوان"
        );

        // Act & Assert
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor("ContactInformation");
    }

    #endregion

    #region Complete Valid Customer Tests

    [Theory]
    [MemberData(nameof(GetValidCustomerTestData))]
    public void CustomerRequest_WhenAllFieldsValid_ShouldPassAllValidations(CustomerRequest request)
    {
        // Act & Assert
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidCustomerTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new CustomerRequest(
                    "محمد",
                    "الخالد",
                    "mohamed@example.com",
                    "0123456789",
                    "الرياض، السعودية"
                )
            },
            new object[]
            {
                new CustomerRequest(
                    "أحمد بن علي",
                    "العتيبي",
                    null,
                    "+966551234567",
                    "جدة، حي السلامة"
                )
            },
            new object[]
            {
                new CustomerRequest(
                    "John",
                    "Smith",
                    "john.smith@company.co.uk",
                    null,
                    "123 Main St, London, UK"
                )
            },
            new object[]
            {
                new CustomerRequest(
                    "سارة",
                    "الراشد",
                    "sara.alrashed@domain.com",
                    "966112345678",
                    null
                )
            },
            new object[]
            {
                new CustomerRequest(
                    "Khaled",
                    "Al-Mansour",
                    "khaled@example.com",
                    "+966551112222",
                    "Riyadh, Saudi Arabia"
                )
            },
            new object[]
            {
                new CustomerRequest(
                    "Mary",
                    "O'Connor",
                    "mary@example.com",
                    null,
                    "Dublin, Ireland"
                )
            }
        };
    }

    #endregion

    #region Mixed Language Name Combinations

    [Theory]
    [MemberData(nameof(GetMixedLanguageNameTestData))]
    public void Names_WhenMixedArabicAndEnglish_ShouldBeValid(string firstName, string lastName)
    {
        // Arrange
        var request = new CustomerRequest(
            firstName,
            lastName,
            "test@example.com",
            "0123456789",
            "عنوان"
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        result.ShouldNotHaveValidationErrorFor(x => x.LastName);
    }
    public static IEnumerable<object[]> GetMixedLanguageNameTestData()
    {
        return new List<object[]>
        {
            new object[] { "عبد الله", "Johnson" },
            new object[] { "John", "الخالد" },
            new object[] { "محمد", "Al-Ghamdi" },
            new object[] { "Ahmed", "O'Brien" },
            new object[] { "سارة", "Smith" },
            new object[] { "Fatima", "العتيبي" }
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Theory]
    [InlineData("", "الخالد", "test@example.com", "0123456789", "عنوان", "الاسم الأول مطلوب.")]
    [InlineData("محمد", "", "test@example.com", "0123456789", "عنوان", "اسم العائلة مطلوب.")]
    [InlineData("محمد", "الخالد", "invalid-email", null, "عنوان", "صيغة البريد الإلكتروني غير صالحة.")]
    public void Validation_ShouldStopOnFirstError_ForCascadingRules(
        string? firstName, string? lastName, string? email, string? phone, string? address, string expectedFirstError)
    {
        // Arrange
        var request = new CustomerRequest(firstName!, lastName!, email!, phone!, address!);

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        Assert.NotEmpty(result.Errors);
        var firstError = result.Errors[0].ErrorMessage;
        Assert.Equal(expectedFirstError, firstError);
    }

    #endregion

    #region Helper Methods

    private static CustomerRequest CreateValidCustomerRequest()
    {
        return new CustomerRequest(
            FirstName: "محمد",
            LastName: "الخالد",
            Email: "test@example.com",
            Phone: "0123456789",
            Address: "عنوان"
        );
    }

    #endregion
}