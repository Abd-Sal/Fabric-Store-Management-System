namespace FabricesStoreManagementSystem.Validations;

public class CatalogValidations : AbstractValidator<CatalogRequest>
{
    private const int MAX_ITEMS = 100;
    private const int MIN_ITEMS = 1;

    public CatalogValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Description validation (optional)
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.")
            .When(x => x.Description != null); // Only check NotEmpty if not null

        RuleFor(x => x.Description)
            .MaximumLength(CatalogConfigurations.DescriptionMaxLenght)
            .WithMessage($"الوصف لا يمكن أن يتجاوز {CatalogConfigurations.DescriptionMaxLenght} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Description)
            .Matches(@"^[\p{IsArabic}a-zA-Z0-9\s\-\.\,\!\?]+$")
            .WithMessage("الوصف يحتوي على أحرف غير مسموح بها.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        // Items validation
        RuleFor(x => x.Items)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("قائمة المنتجات مطلوبة.")
            .NotEmpty()
            .WithMessage("يجب أن يحتوي الكتالوج على منتج واحد على الأقل.")
            .Must(items => items?.Count >= MIN_ITEMS)
            .WithMessage($"يجب أن يحتوي الكتالوج على {MIN_ITEMS} منتج على الأقل.")
            .Must(items => items?.Count <= MAX_ITEMS)
            .WithMessage($"لا يمكن أن يحتوي الكتالوج على أكثر من {MAX_ITEMS} منتج.")
            .Must(HaveUniqueProductIds)
            .WithMessage("يوجد منتجات مكررة في الكتالوج.")
            .Must(HaveReasonableTotalQuantity)
            .WithMessage("إجمالي كمية المنتجات في الكتالوج كبير جدًا.");

        // Validate each catalog product
        RuleForEach(x => x.Items)
            .SetValidator(new CatalogProductValidations());
    }

    private bool HaveUniqueProductIds(List<CatalogProductRequest> items)
    {
        if (items == null) return true;
        var productIds = items.Select(x => x.ProductID).ToList();
        return productIds.Distinct().Count() == productIds.Count;
    }

    private bool HaveReasonableTotalQuantity(List<CatalogProductRequest> items)
    {
        if (items == null) return true;

        const decimal MAX_TOTAL_QUANTITY = 1000m;
        var totalQuantity = items.Sum(item => item.Quantity);
        return totalQuantity <= MAX_TOTAL_QUANTITY;
    }
}