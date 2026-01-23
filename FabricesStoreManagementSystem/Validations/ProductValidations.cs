namespace FabricesStoreManagementSystem.Validations;

public class ProductValidations : AbstractValidator<ProductRequest>
{
    // Predefined valid values (could come from database or configuration)
    private static readonly HashSet<string> ValidUnits = new(StringComparer.OrdinalIgnoreCase)
    {
        "قطعة", "كيلوغرام", "غرام", "لتر", "متر", "علبة", "كرتون", "زوج"
    };

    public ProductValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Name validation (optional)
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("اسم المنتج مطلوب.")
            .When(x => x.Name != null) // Only validate if provided
            .Length(1, ProductConfigurations.NameMaxLength)
            .WithMessage($"اسم المنتج يجب أن يكون بين 1 و {ProductConfigurations.NameMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}\s\-\.\,\d]+$")
            .WithMessage("اسم المنتج يمكن أن يحتوي على أحرف عربية وأرقام ومسافات فقط.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("اسم المنتج لا يمكن أن يكون فارغًا أو مسافات فقط.");

        // Code validation (required)
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("كود المنتج مطلوب.")
            .Length(1, ProductConfigurations.CodeMaxLength)
            .WithMessage($"كود المنتج يجب أن يكون بين 1 و {ProductConfigurations.CodeMaxLength} حرفًا.")
            .Matches(@"^[A-Za-z0-9\-_]+$")
            .WithMessage("كود المنتج يمكن أن يحتوي على أحرف إنجليزية وأرقام وشرطات فقط.")
            .Must(code => !code.Contains(" "))
            .WithMessage("كود المنتج لا يمكن أن يحتوي على مسافات.");

        // Color validation (required)
        RuleFor(x => x.Color)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("لون المنتج مطلوب.")
            .Length(1, ProductConfigurations.ColorMaxLength)
            .WithMessage($"لون المنتج يجب أن يكون بين 1 و {ProductConfigurations.ColorMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}\s]+$")
            .WithMessage("لون المنتج يجب أن يكون باللغة العربية.");

        // Unit validation (required)
        RuleFor(x => x.Unit)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("وحدة القياس مطلوبة.")
            .Length(1, ProductConfigurations.UnitMaxLength)
            .WithMessage($"وحدة القياس يجب أن تكون بين 1 و {ProductConfigurations.UnitMaxLength} حرفًا.")
            .Must(unit => ValidUnits.Contains(unit.Trim()))
            .WithMessage("وحدة القياس غير صالحة. يرجى اختيار وحدة من القائمة المحددة.")
            .Matches(@"^[\p{IsArabic}\s]+$")
            .WithMessage("وحدة القياس يجب أن تكون باللغة العربية.");

        // Material validation (optional)
        RuleFor(x => x.Material)
            .NotEmpty()
            .WithMessage("مادة المنتج مطلوبة.")
            .When(x => x.Material != null) // Only validate if provided
            .Length(1, ProductConfigurations.MaterialMaxLength)
            .WithMessage($"مادة المنتج يجب أن تكون بين 1 و {ProductConfigurations.MaterialMaxLength} حرفًا.")
            .Matches(@"^[\p{IsArabic}\s\-\.\,]+$")
            .WithMessage("مادة المنتج يمكن أن تحتوي على أحرف عربية ومسافات فقط.");
    }
}