namespace FabricesStoreManagementSystem.Validations;

public class CustomerValidations : AbstractValidator<CustomerRequest>
{
    public CustomerValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        #region FirstName Validations
        // First Name validation
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("الاسم الأول مطلوب.")
            .Length(1, CustomerConfigurations.FirstNameMaxLength)
            .WithMessage($"الاسم الأول يجب أن يكون بين 1 و {CustomerConfigurations.FirstNameMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}a-zA-Z\s\-\.']+$")
            .WithMessage("الاسم يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("الاسم الأول لا يمكن أن يكون فارغًا أو مسافات فقط.")
            .Must(name => name.Trim().Length >= 2)
            .WithMessage("الاسم الأول يجب أن يحتوي على حرفين على الأقل.");
        #endregion

        #region LastName Validatoins
        // Last Name validation
        RuleFor(x => x.LastName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("اسم العائلة مطلوب.")
            .Length(1, CustomerConfigurations.LastNameMaxLength)
            .WithMessage($"اسم العائلة يجب أن يكون بين 1 و {CustomerConfigurations.LastNameMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}a-zA-Z\s\-\.']+$")
            .WithMessage("اسم العائلة يمكن أن يحتوي على أحرف عربية وإنجليزية ومسافات وشرطات ونقاط فقط.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("اسم العائلة لا يمكن أن يكون فارغًا أو مسافات فقط.");
        #endregion

        #region Email Validatoins
        // Email validation (optional but must be valid if provided)
        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("البريد الإلكتروني لا يمكن أن يكون فارغًا إذا تم تقديمه.")
                .Length(1, CustomerConfigurations.EmailMaxLength)
                .WithMessage($"البريد الإلكتروني يجب أن يكون بين 1 و {CustomerConfigurations.EmailMaxLength} حرفًا.")
                .EmailAddress()
                .WithMessage("صيغة البريد الإلكتروني غير صالحة.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")
                .WithMessage("صيغة البريد الإلكتروني غير صحيحة.");
        });
        #endregion

        #region Address Validatoins
        // Address validation (optional)
        When(x => !string.IsNullOrWhiteSpace(x.Address), () =>
        {
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("العنوان لا يمكن أن يكون فارغًا إذا تم تقديمه.")
                .Length(1, CustomerConfigurations.AddressMaxLength)
                .WithMessage($"العنوان يجب أن يكون بين 1 و {CustomerConfigurations.AddressMaxLength} حرفًا.")
                .Matches(@"^[\p{IsArabic}\p{IsBasicLatin}\s\-\.,\d]+$")
                .WithMessage("العنوان يحتوي على أحرف غير مسموح بها.");
        });
        #endregion

        #region Phone Validatoins
        // Phone validation (optional but must be valid if provided) - FIXED TYPO
        When(x => !string.IsNullOrWhiteSpace(x.Phone), () =>
        {
            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("رقم الهاتف لا يمكن أن يكون فارغًا إذا تم تقديمه.")
                .Length(1, CustomerConfigurations.PhoneMaxLength)
                .WithMessage($"رقم الهاتف يجب أن يكون بين 1 و {CustomerConfigurations.PhoneMaxLength} حرفًا.")
                .Matches(@"^[\d\s\-\+\(\)]+$")
                .WithMessage("رقم الهاتف يمكن أن يحتوي على أرقام ومسافات وشرطات وعلامة الجمع وأقواس فقط.")
                .Must(IsValidPhoneNumber)
                .WithMessage("رقم الهاتف غير صالح.");
        });
        #endregion

        // Business rule: At least one contact method (email or phone) is required
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