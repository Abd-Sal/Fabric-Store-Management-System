namespace FabricesStoreManagementSystem.Validations;

public class CatalogFromSupplierValidations : AbstractValidator<CatalogFromSupplierRequest>
{
    private const int MAX_ITEMS = 100;

    public CatalogFromSupplierValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Supplier ID validation
        RuleFor(x => x.SupplierID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("معرف المورد مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المورد لا يمكن أن يكون فارغًا.");

        // Description validation (optional)
        RuleFor(x => x.Description)
            .MaximumLength(CatalogConfigurations.DescriptionMaxLenght)
            .WithMessage($"الوصف لا يمكن أن يتجاوز {CatalogConfigurations.DescriptionMaxLenght} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .Must(desc => desc == null || !string.IsNullOrWhiteSpace(desc))
            .WithMessage("الوصف لا يمكن أن يكون فارغًا أو مسافات فقط.")
            .When(x => x.Description != null);

        // Items validation (list of product IDs)
        RuleFor(x => x.Items)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("قائمة المنتجات مطلوبة.")
            .NotEmpty()
            .WithMessage("يجب أن تحتوي القائمة على منتج واحد على الأقل.")
            .Must(items => items?.Count > 0)
            .WithMessage("يجب إضافة منتجات إلى الكتالوج.")
            .Must(HaveUniqueProductIds)
            .WithMessage("يوجد معرفات منتجات مكررة في القائمة.")
            .Must(HaveReasonableItemCount)
            .WithMessage($"لا يمكن أن تحتوي القائمة على أكثر من {MAX_ITEMS} منتج.")
            .Must(NotContainEmptyGuids)
            .WithMessage("القائمة تحتوي على معرفات منتجات فارغة.");
    }

    private bool HaveUniqueProductIds(List<Guid> items)
    {
        if (items == null) return true;
        return items.Distinct().Count() == items.Count;
    }

    private bool HaveReasonableItemCount(List<Guid> items)
    {
        return items?.Count <= MAX_ITEMS;
    }

    private bool NotContainEmptyGuids(List<Guid> items)
    {
        if (items == null) return true;
        return !items.Any(id => id == Guid.Empty);
    }
}