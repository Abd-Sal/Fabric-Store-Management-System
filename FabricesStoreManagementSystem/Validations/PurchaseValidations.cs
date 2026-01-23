namespace FabricesStoreManagementSystem.Validations;

public class PurchaseValidations : AbstractValidator<PurchaseRequest>
{
    private const decimal MAX_PURCHASE_AMOUNT = 1000000m;
    private const int MAX_DECIMAL_PLACES = 2;

    public PurchaseValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Supplier validation
        RuleFor(x => x.SupplierID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("معرف المورد مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المورد لا يمكن أن يكون فارغًا.");

        // Paid amount validation
        RuleFor(x => x.PaidAmount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("المبلغ المدفوع مطلوب.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("المبلغ المدفوع لا يمكن أن يكون سالبًا.")
            .Must(HaveValidCurrencyPrecision)
            .WithMessage($"المبلغ المدفوع يمكن أن يحتوي على حد أقصى {MAX_DECIMAL_PLACES} منازل عشرية.")
            .Must(BeValidCurrencyAmount)
            .WithMessage("المبلغ المدفوع يجب أن يكون مضاعفًا للـ 0.01.");

        // Purchase items validation
        RuleFor(x => x.PurchaseItems)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("عناصر الشراء مطلوبة.")
            .NotEmpty()
            .WithMessage("يجب أن تحتوي على عنصر شراء واحد على الأقل.")
            .Must(items => items?.Count > 0)
            .WithMessage("قائمة عناصر الشراء لا يمكن أن تكون فارغة.")
            .Must(HaveUniqueProductIds)
            .WithMessage("تم العثور على معرفات منتجات مكررة في عناصر الشراء.")
            .Must(HaveReasonableItemCount)
            .WithMessage("عدد كبير جدًا من عناصر الشراء في معاملة واحدة.");

        // Validate each purchase item
        RuleForEach(x => x.PurchaseItems)
            .SetValidator(new PurchaseItemValidations());

        // Cross-validation: Paid amount should not exceed total cost
        RuleFor(x => x)
            .Must(x => x.PaidAmount <= CalculateTotalCost(x.PurchaseItems))
            .WithMessage("المبلغ المدفوع يتجاوز التكلفة الإجمالية للشراء.")
            .When(x => x.PurchaseItems != null && x.PurchaseItems.Any());

        // Business rule: Maximum purchase amount
        RuleFor(x => x)
            .Must(x => CalculateTotalCost(x.PurchaseItems) <= MAX_PURCHASE_AMOUNT)
            .WithMessage($"التكلفة الإجمالية للشراء لا يمكن أن تتجاوز {MAX_PURCHASE_AMOUNT:C}.")
            .When(x => x.PurchaseItems != null && x.PurchaseItems.Any());

        // Business rule: Minimum purchase amount (optional)
        RuleFor(x => x)
            .Must(x => CalculateTotalCost(x.PurchaseItems) >= 0.01m)
            .WithMessage("الحد الأدنى لقيمة الشراء هو 0.01.")
            .When(x => x.PurchaseItems != null && x.PurchaseItems.Any());
    }

    private bool HaveValidCurrencyPrecision(decimal amount)
    {
        var bits = decimal.GetBits(amount);
        int scale = (bits[3] >> 16) & 0x7F;
        return scale <= MAX_DECIMAL_PLACES;
    }

    private bool BeValidCurrencyAmount(decimal amount)
    {
        return amount % 0.01m == 0;
    }

    private bool HaveUniqueProductIds(List<PurchaseItemRequest> purchaseItems)
    {
        if (purchaseItems == null) return true;
        var productIds = purchaseItems.Select(x => x.ProductID).ToList();
        return productIds.Distinct().Count() == productIds.Count;
    }

    private bool HaveReasonableItemCount(List<PurchaseItemRequest> purchaseItems)
    {
        const int MAX_ITEMS_PER_PURCHASE = 100;
        return purchaseItems?.Count <= MAX_ITEMS_PER_PURCHASE;
    }

    private decimal CalculateTotalCost(List<PurchaseItemRequest> purchaseItems)
    {
        if (purchaseItems == null || !purchaseItems.Any())
            return 0;

        return purchaseItems.Sum(item => (decimal)item.Quantity * item.UnitCost);
    }
}