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
        When(x => x.Name != null, () =>
        {
            RuleFor(x => x.Name)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("مادة الصنع لا يمكن أن تكون فارغة أو مسافات فقط.")
                .Length(1, ProductConfigurations.NameMaxLength)
                .WithMessage($"اسم المنتج يجب أن يكون بين 1 و {ProductConfigurations.NameMaxLength} حرفًا.");
        });

        // Code validation (required)
        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("كود المنتج مطلوب.")
            .Length(1, ProductConfigurations.CodeMaxLength)
            .WithMessage($"كود المنتج يجب أن يكون بين 1 و {ProductConfigurations.CodeMaxLength} حرفًا.")
            .Must(code => !code.Contains(" "))
            .WithMessage("كود المنتج لا يمكن أن يحتوي على مسافات.");

        // Color validation (required)
        RuleFor(x => x.Color)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("لون المنتج مطلوب.")
            .Length(1, ProductConfigurations.ColorMaxLength)
            .WithMessage($"لون المنتج يجب أن يكون بين 1 و {ProductConfigurations.ColorMaxLength} حرفًا.");

        // Unit validation (required)
        RuleFor(x => x.Unit)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("وحدة القياس مطلوبة.")
            .Length(1, ProductConfigurations.UnitMaxLength)
            .WithMessage($"وحدة القياس يجب أن تكون بين 1 و {ProductConfigurations.UnitMaxLength} حرفًا.")
            .Must(unit => ValidUnits.Contains(unit.Trim()))
            .WithMessage("وحدة القياس غير صالحة. يرجى اختيار وحدة من القائمة المحددة.");

        // Material validation (optional)
        When(x => x.Material != null, () =>
        {
            RuleFor(x => x.Material)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("مادة المنتج مطلوبة.")
                .When(x => x.Material != null) // Only validate if provided
                .Length(1, ProductConfigurations.MaterialMaxLength)
                .WithMessage($"مادة المنتج يجب أن تكون بين 1 و {ProductConfigurations.MaterialMaxLength} حرفًا.");
        });
    }
}