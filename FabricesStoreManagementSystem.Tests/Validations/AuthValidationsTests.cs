namespace FabricesStoreManagementSystem.Tests.Validations;

public class AuthValidationsTests
{
    private readonly AuthValidations _validator = new();

    private static LoginRequest CreateValidLoginRequest() => new(
        Username: "john.doe@example.com",
        Password: "SecurePass123!"
    );

    #region Username Validation Tests

    [Theory]
    [InlineData(null, "اسم المستخدم مطلوب.")]
    [InlineData("", "اسم المستخدم مطلوب.")]
    [InlineData("   ", "اسم المستخدم مطلوب.")]
    public void Username_WhenNullOrWhiteSpace_ShouldHaveValidationError(string username, string expectedError)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("ab")] // Too short
    [InlineData("a")]  // Too short
    public void Username_WhenTooShort_ShouldHaveValidationError(string username)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("اسم المستخدم يجب أن يكون بين 3 و 50 حرفًا.");
    }

    [Fact]
    public void Username_WhenTooLong_ShouldHaveValidationError()
    {
        var longUsername = new string('a', 51);
        var request = CreateValidLoginRequest() with { Username = longUsername };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("اسم المستخدم يجب أن يكون بين 3 و 50 حرفًا.");
    }

    [Theory]
    [InlineData("user name")]    // Contains space
    [InlineData("أحمد")]         // Arabic letters
    [InlineData("user#name")]    // Contains #
    [InlineData("user%name")]    // Contains %
    [InlineData("user&name")]    // Contains &
    public void Username_WhenContainsInvalidCharacters_ShouldHaveValidationError(string username)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("اسم المستخدم يمكن أن يحتوي على أحرف إنجليزية وأرقام ونقاط وشرطات وشرطات سفلية وعلامة @ فقط.");
    }

    [Theory]
    [InlineData(".username")]    // Starts with dot
    [InlineData("username.")]    // Ends with dot
    public void Username_WhenStartsOrEndsWithDot_ShouldHaveValidationError(string username)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("اسم المستخدم لا يمكن أن يبدأ أو ينتهي بنقطة.");
    }

    [Theory]
    [InlineData("user@@domain")]    // Multiple @ symbols
    [InlineData("user@name@domain")] // Multiple @ symbols
    public void Username_WhenMultipleAtSymbols_ShouldHaveValidationError(string username)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage("اسم المستخدم لا يمكن أن يحتوي على أكثر من علامة @ واحدة.");
    }

    [Theory]
    [InlineData("john.doe")]
    [InlineData("jane_doe")]
    [InlineData("user-name")]
    [InlineData("user@domain")]
    [InlineData("user123")]
    [InlineData("admin.user@company.com")]
    public void Username_WhenValid_ShouldNotHaveValidationError(string username)
    {
        var request = CreateValidLoginRequest() with { Username = username };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    #endregion

    #region Password Validation Tests

    [Theory]
    [InlineData(null, "كلمة المرور مطلوبة.")]
    [InlineData("", "كلمة المرور مطلوبة.")]
    [InlineData("   ", "كلمة المرور مطلوبة.")]
    public void Password_WhenNullOrWhiteSpace_ShouldHaveValidationError(string password, string expectedError)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(expectedError);
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Password_WhenTooShort_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور يجب أن تحتوي على 8 أحرف على الأقل.");
    }

    [Fact]
    public void Password_WhenTooLong_ShouldHaveValidationError()
    {
        var longPassword = new string('a', 101);
        var request = CreateValidLoginRequest() with { Password = longPassword };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور لا يمكن أن تتجاوز 100 حرفًا.");
    }

    [Theory]
    [InlineData("alllowercase123!")]
    [InlineData("12345678!")]
    public void Password_WhenNoUppercase_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل.");
    }

    [Theory]
    [InlineData("ALLUPPERCASE123!")]
    [InlineData("PASSWORD123!")]
    public void Password_WhenNoLowercase_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل.");
    }

    [Theory]
    [InlineData("NoDigitsHere!")]
    [InlineData("Password!")]
    public void Password_WhenNoDigits_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.");
    }

    [Theory]
    [InlineData("NoSpecial123")]
    [InlineData("Password123")]
    public void Password_WhenNoSpecialCharacters_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل (!@#$%^&*).");
    }

    [Theory]
    [InlineData("password")]
    [InlineData("123456")]
    [InlineData("admin")]
    [InlineData("كلمة السر")]
    public void Password_WhenExactBannedPassword_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("aaaaaaaa")]
    [InlineData("11111111")]
    [InlineData("11223344")]
    [InlineData("qwertyui")]
    [InlineData("asdfghjk")]
    public void Password_WhenSimplePattern_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Password_WhenContainsUsername_ShouldHaveValidationError()
    {
        var request = new LoginRequest(
            Username: "john",
            Password: "MyPasswordjohn123!"
        );

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور لا يمكن أن تحتوي على اسم المستخدم.");
    }

    [Theory]
    [InlineData("SecurePass123!")]
    [InlineData("MyP@ssw0rd")]
    [InlineData("Test@2024")]
    [InlineData("HelloWorld123!")]
    public void Password_WhenValid_ShouldNotHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    #endregion

    #region SQL Injection Pattern Tests for Password

    [Theory]
    [InlineData("Secure123'; DROP TABLE users;--")]
    [InlineData("Password123' OR '1'='1")]
    [InlineData("Test123'; SELECT * FROM users;")]
    public void Password_WhenValidButContainsSqlInjection_ShouldHaveValidationError(string password)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور تحتوي على أنماط غير مسموح بها.");
    }

    // Or test the basic failing case separately
    [Theory]
    [InlineData("pass'; DROP TABLE users;--", "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.")]
    [InlineData("test'; SELECT * FROM users;", "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل.")]
    public void Password_WhenInvalidAndContainsSqlInjection_ShouldHaveFirstError(string password, string expectedError)
    {
        var request = CreateValidLoginRequest() with { Password = password };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage(expectedError);
    }

    #endregion

    #region Complete Valid Login Tests

    [Theory]
    [MemberData(nameof(GetValidLoginTestData))]
    public void LoginRequest_WhenValid_ShouldPassAllValidations(LoginRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidLoginTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new LoginRequest("john.doe@company.com", "SecurePass123!")
            },
            new object[]
            {
                new LoginRequest("user_123", "P@ssw0rd2024")
            },
            new object[]
            {
                new LoginRequest("admin.user", "Admin@Secure123")
            },
            new object[]
            {
                new LoginRequest("test-user", "Test@Password123")
            }
        };
    }

    #endregion

    #region Cascade Mode Tests

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForUsername()
    {
        var request = CreateValidLoginRequest() with { Username = "" };

        var result = _validator.TestValidate(request);

        var usernameErrors = result.Errors
            .Where(e => e.PropertyName == "Username")
            .ToList();

        Assert.Single(usernameErrors);
        Assert.Equal("اسم المستخدم مطلوب.", usernameErrors[0].ErrorMessage);
    }

    [Fact]
    public void Validation_ShouldStopOnFirstError_ForPassword()
    {
        var request = CreateValidLoginRequest() with { Password = "" };

        var result = _validator.TestValidate(request);

        var passwordErrors = result.Errors
            .Where(e => e.PropertyName == "Password")
            .ToList();

        Assert.Single(passwordErrors);
        Assert.Equal("كلمة المرور مطلوبة.", passwordErrors[0].ErrorMessage);
    }

    #endregion

    #region Password Complexity Edge Cases
    [Fact]
    public void Password_WhenUsernameInPassword_ShouldFailCustomValidation()
    {
        var request = new LoginRequest(
            Username: "admin",
            Password: "AdminPassword123!"
        );

        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("كلمة المرور لا يمكن أن تحتوي على اسم المستخدم.");
    }

    #endregion
}