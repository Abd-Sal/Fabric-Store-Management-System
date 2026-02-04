namespace FabricesStoreManagementSystem.Validations;

public class SupplierValidations : AbstractValidator<SupplierRequest>
{
    public SupplierValidations()
    {
        #region Name Validations
        // Name validation
        RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("اسم المورد مطلوب.")
                .Length(3, SupplierConfigurations.NameMaxLength)
                .WithMessage($"الاسم يجب أن يكون بين 3 و {SupplierConfigurations.NameMaxLength} حرفًا.")
                .Matches(@"^[\p{IsArabic}a-zA-Z0-9\s\-\.&',]+$")
                .WithMessage("الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية وأرقام ومسافات وعلامات ترقيم أساسية فقط.")
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("الاسم لا يمكن أن يكون فارغًا أو مسافات فقط.")
                .Must(name => name.Trim().Length >= 3)
                .WithMessage("الاسم يجب أن يحتوي على 3 أحرف على الأقل.");
        #endregion

        #region Email Validations
        // Email validation (if provided)
        // Rule 1: If email is provided, it shouldn't be whitespace-only
        RuleFor(x => x.Email)
            .Must(email => email == null || !string.IsNullOrWhiteSpace(email))
            .WithMessage("البريد الإلكتروني لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .When(x => x.Email != null);

        // Rule 2: Length validation (only for valid, non-whitespace emails)
        RuleFor(x => x.Email)
            .Length(3, SupplierConfigurations.EmailMaxLength)
            .WithMessage($"البريد الإلكتروني يجب أن يكون بين 3 و {SupplierConfigurations.EmailMaxLength} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Rule 3: Email format validation using FluentValidation's built-in validator
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("يجب إدخال بريد إلكتروني صالح.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        // Rule 4: Additional regex validation for email format
        RuleFor(x => x.Email)
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .WithMessage("صيغة البريد الإلكتروني غير صحيحة.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        #endregion

        #region Address Validations
        // Address validation (if provided)
        RuleFor(x => x.Address)
            .Must(address => address == null || !string.IsNullOrWhiteSpace(address))
            .WithMessage("العنوان لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .When(x => x.Address != null);

        // Rule 2: Length validation (only for valid addresses)
        RuleFor(x => x.Address)
            .Length(1, SupplierConfigurations.AddressMaxLength)
            .WithMessage($"العنوان يجب أن يكون بين 1 و {SupplierConfigurations.AddressMaxLength} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        // Rule 3: Format validation (only for valid addresses)
        RuleFor(x => x.Address)
            .Matches(@"^[\p{IsArabic}a-zA-Z0-9\s\-\.\,\#\&]+$")
            .WithMessage("العنوان يحتوي على أحرف غير مسموح بها.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));
        #endregion

        #region Phone Validations
        // Phone validation (if provided)
        // Rule 1: If phone is provided, it shouldn't be whitespace-only
        RuleFor(x => x.Phone)
            .Must(phone => phone == null || !string.IsNullOrWhiteSpace(phone))
            .WithMessage("رقم الهاتف لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .When(x => x.Phone != null);

        // Rule 2: Length validation (only for valid, non-whitespace phone numbers)
        RuleFor(x => x.Phone)
            .Length(1, SupplierConfigurations.PhoneMaxLength)
            .WithMessage($"رقم الهاتف يجب أن يكون بين 1 و {SupplierConfigurations.PhoneMaxLength} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        // Rule 3: Character validation - allowed characters only
        RuleFor(x => x.Phone)
            .Matches(@"^[\d\s\-\+\(\)]+$")
            .WithMessage("رقم الهاتف يمكن أن يحتوي على أرقام ومسافات وشرطات وعلامة الجمع وأقواس فقط.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        // Rule 4: Custom validation for phone number format
        RuleFor(x => x.Phone)
            .Must(phone => IsValidPhoneNumber(phone))
            .WithMessage("صيغة رقم الهاتف غير صالحة.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
        #endregion

        // Business rule: At least one contact method (email or phone) should be required
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("يجب تقديم وسيلة اتصال واحدة على الأقل (بريد إلكتروني أو رقم هاتف).")
            .WithName("ContactInformation");
    }

    private bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return true;
        // Remove all non-digit characters for validation
        var digitsOnly = Regex.Replace(phone, @"[^\d]", "");
        // Basic validation: at least 7 digits for a phone number
        return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
    }
}