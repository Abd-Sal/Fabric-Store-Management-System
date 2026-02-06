namespace FabricesStoreManagementSystem.Validations;

public class CatalogFormPurchaseCatalogValidations : AbstractValidator<CatalogFormPurchaseCatalogRequest>
{
    public CatalogFormPurchaseCatalogValidations()
    {
        RuleFor(x => x.SupplierID)
            .NotEmpty().WithMessage("معرف المورد مطلوب")
            .NotEqual(Guid.Empty).WithMessage("معرف المورد غير صالح");

        RuleFor(x => x.Description)
            .MinimumLength(1)
            .WithMessage("يجب أن يتجاوز الوصف 1 حرف على الأقل")
            .When(x => !string.IsNullOrEmpty(x.Description))
            .MaximumLength(500)
            .WithMessage("يجب ألا يتجاوز الوصف 500 حرف")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("يجب إضافة عناصر على الأقل")
            .Must(items => items != null && items.Count > 0).WithMessage("قائمة العناصر لا يمكن أن تكون فارغة")
            .Must(items => items.All(item => item != Guid.Empty)).WithMessage("يحتوي أحد العناصر على معرف غير صالح");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("يجب أن يكون المبلغ أكبر من صفر")
            .PrecisionScale(18, 2, false).WithMessage("يجب أن يحتوي المبلغ على منزلتين عشريتين على الأكثر");

        RuleFor(x => x.PaidAmount)
            .GreaterThanOrEqualTo(0).WithMessage("يجب أن تكون القيمة المدفوعة أكبر من أو تساوي صفر")
            .PrecisionScale(18, 2, false).WithMessage("يجب أن تحتوي القيمة المدفوعة على منزلتين عشريتين على الأكثر")
            .LessThanOrEqualTo(x => x.Amount).WithMessage("القيمة المدفوعة لا يمكن أن تتجاوز المبلغ الإجمالي");

        RuleFor(x => x)
            .Must(x => x.PaidAmount <= x.Amount).WithMessage("القيمة المدفوعة لا يمكن أن تتجاوز المبلغ الإجمالي")
            .When(x => x.PaidAmount > 0 && x.Amount > 0);
    }
}