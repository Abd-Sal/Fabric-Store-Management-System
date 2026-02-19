namespace FabricesStoreManagementSystem.Tests.Validations;

public class SupplierValidationsTests
{
    private readonly SupplierValidations _validator = new();

    private static SupplierRequest CreateValidSupplierRequest() => new(
        Name: "مورد الإلكترونيات المتكامل",
        Email: "supplier@example.com",
        Phone: "+966 55 123 4567",
        Address: "الرياض، المملكة العربية السعودية"
    );

    #region Name Validation Tests

    [Fact]
    public void Name_WhenTooShort_ShouldHaveValidationError()
    {
        var request = CreateValidSupplierRequest() with { Name = "ab" };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يجب أن يحتوي على 3 أحرف على الأقل.");
    }

    [Fact]
    public void Name_WhenTooLong_ShouldHaveValidationError()
    {
        var longName = new string('أ', SupplierConfigurations.NameMaxLength + 1);
        var request = CreateValidSupplierRequest() with { Name = longName };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage($"الاسم يجب أن يكون بين 3 و {SupplierConfigurations.NameMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("Supplier@2024")] // @ not allowed
    [InlineData("مورد#1")] // # not in allowed punctuation
    [InlineData("Company*Ltd")] // * not allowed
    [InlineData("Test!Company")] // ! not allowed
    public void Name_WhenContainsInvalidCharacters_ShouldHaveValidationError(string name)
    {
        var request = CreateValidSupplierRequest() with { Name = name };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية وأرقام ومسافات وعلامات ترقيم أساسية فقط.");
    }

    [Theory]
    [InlineData("مورد الإلكترونيات")]
    [InlineData("ABC Electronics Co.")]
    [InlineData("Al-Othaim Markets")]
    [InlineData("Saudi Aramco")]
    [InlineData("شركة محمد وعلي للمقاولات")]
    [InlineData("Tech Solutions & Services")]
    [InlineData("O'Reilly Suppliers")]
    [InlineData("AT&T Supplier Network")]
    public void Name_WhenValid_ShouldNotHaveValidationError(string name)
    {
        var request = CreateValidSupplierRequest() with { Name = name };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    #endregion

    #region Email Validation Tests

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void Email_WhenProvidedButWhitespace_ShouldHaveValidationError(string email)
    {
        var request = CreateValidSupplierRequest() with { Email = email };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("البريد الإلكتروني لا يمكن أن يكون فارغًا إذا تم تقديمه.");
    }

    [Fact]
    public void Email_WhenTooLong_ShouldHaveValidationError()
    {
        var longEmail = "a".PadRight(SupplierConfigurations.EmailMaxLength - 10, 'a') + "@example.com";
        var request = CreateValidSupplierRequest() with { Email = longEmail };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage($"البريد الإلكتروني يجب أن يكون بين 3 و {SupplierConfigurations.EmailMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@no-username.com")]
    [InlineData("no-domain@")]
    [InlineData("spaces in@email.com")]
    public void Email_WhenInvalidFormat_ShouldHaveValidationError(string email)
    {
        var request = CreateValidSupplierRequest() with { Email = email };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    public void Email_WhenNotProvided_ShouldNotHaveValidationError(string? email)
    {
        var request = CreateValidSupplierRequest() with { Email = email };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("supplier@example.com")]
    [InlineData("contact@company.co.uk")]
    [InlineData("مورد.أجهزة@example.com")]
    [InlineData("name.surname@sub.domain.com")]
    public void Email_WhenValid_ShouldNotHaveValidationError(string email)
    {
        var request = CreateValidSupplierRequest() with { Email = email };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    #endregion

    #region Phone Validation Tests

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void Phone_WhenProvidedButWhitespace_ShouldHaveValidationError(string phone)
    {
        var request = CreateValidSupplierRequest() with { Phone = phone };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage("رقم الهاتف لا يمكن أن يكون فارغًا إذا تم تقديمه.");
    }

    [Fact]
    public void Phone_WhenTooLong_ShouldHaveValidationError()
    {
        var longPhone = new string('1', SupplierConfigurations.PhoneMaxLength + 1);
        var request = CreateValidSupplierRequest() with { Phone = longPhone };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone)
            .WithErrorMessage($"رقم الهاتف يجب أن يكون بين 4 و {SupplierConfigurations.PhoneMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("invalid-phone")]
    [InlineData("abc123")]
    [InlineData("123@456")]
    public void Phone_WhenContainsInvalidCharacters_ShouldHaveValidationError(string phone)
    {
        var request = CreateValidSupplierRequest() with { Phone = phone };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12 34")]
    [InlineData("+++()()()")]
    [InlineData("12345678901234567")]
    public void Phone_WhenInvalidDigitCount_ShouldHaveValidationError(string phone)
    {
        var request = CreateValidSupplierRequest() with { Phone = phone };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData(null)]
    public void Phone_WhenNotProvided_ShouldNotHaveValidationError(string? phone)
    {
        var request = CreateValidSupplierRequest() with { Phone = phone };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("1234567890")]
    [InlineData("+12345678900")]
    [InlineData("0123456789")]
    [InlineData("966112345678")]
    [InlineData("00966112345678")]
    [InlineData("966551234567")]
    public void Phone_WhenValid_ShouldNotHaveValidationError(string phone)
    {
        var request = CreateValidSupplierRequest() with { Phone = phone };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Phone);
    }

    #endregion

    #region Address Validation Tests

    [Theory]
    [InlineData("   ")]
    [InlineData("")]
    public void Address_WhenProvidedButWhitespace_ShouldHaveValidationError(string address)
    {
        var request = CreateValidSupplierRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("العنوان لا يمكن أن يكون فارغًا إذا تم تقديمه.");
    }

    [Fact]
    public void Address_WhenTooLong_ShouldHaveValidationError()
    {
        var longAddress = new string('أ', SupplierConfigurations.AddressMaxLength + 1);
        var request = CreateValidSupplierRequest() with { Address = longAddress };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage($"العنوان يجب أن يكون بين 1 و {SupplierConfigurations.AddressMaxLength} حرفًا.");
    }

    [Theory]
    [InlineData("عنوان é")] // Extended Latin
    [InlineData("Street Дворцовая")] // Cyrillic
    [InlineData("東京住所")] // Japanese
    [InlineData("Address ≠ Street")] // Math symbol
    [InlineData("€ 100 Street")] // Currency
    [InlineData("Copyright© Street")] // Symbol
    public void Address_WhenContainsInvalidCharacters_ShouldHaveValidationError(string address)
    {
        var request = CreateValidSupplierRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("العنوان يحتوي على أحرف غير مسموح بها.");
    }

    [Theory]
    [InlineData(null)]
    public void Address_WhenNotProvided_ShouldNotHaveValidationError(string? address)
    {
        var request = CreateValidSupplierRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData("شارع الملك فهد، الرياض")]
    [InlineData("123 Main Street, New York")]
    [InlineData("حي المروج، جدة")]
    [InlineData("P.O. Box 1234, Dubai")]
    [InlineData("مبنى النخيل، الطابق 3")]
    [InlineData("Industrial Area #45, Dammam")]
    public void Address_WhenValid_ShouldNotHaveValidationError(string address)
    {
        var request = CreateValidSupplierRequest() with { Address = address };
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    #endregion

    #region Contact Information Business Rule Tests

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    [InlineData(null, "   ")]
    [InlineData("   ", null)]
    public void Supplier_WhenNoContactInformationProvided_ShouldHaveValidationError(string? email, string? phone)
    {
        var request = new SupplierRequest(
            "مورد الإلكترونيات",
            email,
            phone,
            "عنوان"
        );
        _validator.TestValidate(request)
            .ShouldHaveValidationErrorFor("ContactInformation")
            .WithErrorMessage("يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).");
    }

    [Theory]
    [InlineData("supplier@example.com", null)]
    [InlineData(null, "+966 55 123 4567")]
    [InlineData("supplier@example.com", "+966 55 123 4567")]
    [InlineData("supplier@example.com", "")]
    [InlineData("", "+966 55 123 4567")]
    public void Supplier_WhenAtLeastOneContactProvided_ShouldNotHaveValidationError(string? email, string? phone)
    {
        var request = new SupplierRequest(
            "مورد الإلكترونيات",
            email,
            phone,
            "عنوان"
        );
        _validator.TestValidate(request)
            .ShouldNotHaveValidationErrorFor("ContactInformation");
    }

    #endregion

    #region Complete Valid Supplier Tests

    [Theory]
    [MemberData(nameof(GetValidSupplierTestData))]
    public void SupplierRequest_WhenAllFieldsValid_ShouldPassAllValidations(SupplierRequest request)
    {
        _validator.TestValidate(request)
            .ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidSupplierTestData()
    {
        return new List<object[]>
        {
            new object[]
            {
                new SupplierRequest(
                    "مورد الإلكترونيات المتكامل",
                    "info@electronic.com",
                    "+966551234567",
                    "الرياض، المملكة العربية السعودية"
                )
            },
            new object[]
            {
                new SupplierRequest(
                    "ABC Construction Materials",
                    null,
                    "0123456789",
                    "123 Industrial Zone, Jeddah"
                )
            },
            new object[]
            {
                new SupplierRequest(
                    "شركة الأغذية المتحدة",
                    "food@united.com",
                    null,
                    "مستودع رقم 5، الدمام"
                )
            },
            new object[]
            {
                new SupplierRequest(
                    "Tech Gear & Equipment",
                    "sales@techgear.com",
                    "966112345678",
                    null
                )
            }
        };
    }

    #endregion
}