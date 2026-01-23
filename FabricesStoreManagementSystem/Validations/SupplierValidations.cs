namespace FabricesStoreManagementSystem.Validations;

public class SupplierValidations : AbstractValidator<SupplierRequest>
{
    public SupplierValidations()
    {
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

        // Email validation (if provided)
        RuleFor(x => x.Email)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("البريد الإلكتروني لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .Length(3, SupplierConfigurations.EmailMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage($"البريد الإلكتروني يجب أن يكون بين 3 و {SupplierConfigurations.EmailMaxLength} حرفًا.")
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("يجب إدخال بريد إلكتروني صالح.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

        // Phone validation (if provided)
        RuleFor(x => x.Phone)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("رقم الهاتف لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .Length(1, SupplierConfigurations.PhoneMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage($"رقم الهاتف يجب أن يكون بين 1 و {SupplierConfigurations.PhoneMaxLength} حرفًا.")
            .Matches(@"^[\d\s\-\+\(\)]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("رقم الهاتف يمكن أن يحتوي على أرقام ومسافات وشرطات وعلامة الجمع وأقواس فقط.")
            .Must(phone => IsValidPhoneNumber(phone))
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("صيغة رقم الهاتف غير صالحة.");

        // Address validation (if provided)
        RuleFor(x => x.Address)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("العنوان لا يمكن أن يكون فارغًا إذا تم تقديمه.")
            .Length(1, SupplierConfigurations.AddressMaxLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage($"العنوان يجب أن يكون بين 1 و {SupplierConfigurations.AddressMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}a-zA-Z0-9\s\-\.\,\#\&]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Address))
            .WithMessage("العنوان يحتوي على أحرف غير مسموح بها.");

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