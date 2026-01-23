namespace FabricesStoreManagementSystem.Validations;

public class SaleValidations : AbstractValidator<SaleRequest>
{
    private const decimal MAX_DISCOUNT_AMOUNT = 10000m;
    private const decimal MIN_SALE_AMOUNT = 0.01m;
    private const int MAX_DECIMAL_PLACES = 2; // For currency amounts

    public SaleValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Customer validation
        RuleFor(x => x.CustomerID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("معرف العميل مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف العميل لا يمكن أن يكون فارغًا.");

        // DISCOUNT AS AMOUNT
        RuleFor(x => x.Discount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("مبلغ الخصم مطلوب.")
            .GreaterThanOrEqualTo(0)
            .WithMessage("مبلغ الخصم لا يمكن أن يكون سالبًا.")
            .LessThanOrEqualTo(MAX_DISCOUNT_AMOUNT)
            .WithMessage($"مبلغ الخصم لا يمكن أن يتجاوز {MAX_DISCOUNT_AMOUNT:C}.")
            .Must(HaveValidCurrencyPrecision)
            .WithMessage($"مبلغ الخصم يمكن أن يحتوي على حد أقصى {MAX_DECIMAL_PLACES} منازل عشرية.")
            .Must(BeValidCurrencyAmount)
            .WithMessage("مبلغ الخصم يجب أن يكون مضاعفًا للـ 0.01.");

        // Paid amount validation - ONLY basic validation since partial payments are allowed
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

        // Sale items validation
        RuleFor(x => x.SaleItems)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("عناصر البيع مطلوبة.")
            .NotEmpty()
            .WithMessage("يجب أن تحتوي على عنصر بيع واحد على الأقل.")
            .Must(items => items?.Count > 0)
            .WithMessage("قائمة عناصر البيع لا يمكن أن تكون فارغة.")
            .Must(HaveUniqueProductIds)
            .WithMessage("تم العثور على معرفات منتجات مكررة في عناصر البيع.")
            .Must(HaveReasonableItemCount)
            .WithMessage("عدد كبير جدًا من عناصر البيع في معاملة واحدة.");

        // Validate each sale item
        RuleForEach(x => x.SaleItems)
            .SetValidator(new SaleItemValidations());

        // Cross-validation: Discount cannot exceed subtotal
        RuleFor(x => x)
            .Must(x => x.Discount <= CalculateSubtotal(x.SaleItems))
            .WithMessage("مبلغ الخصم لا يمكن أن يتجاوز المجموع الفرعي.")
            .When(x => x.SaleItems != null && x.SaleItems.Any());

        // Cross-validation: Net total cannot be negative
        RuleFor(x => x)
            .Must(x => CalculateNetTotal(x.SaleItems, x.Discount) >= 0)
            .WithMessage("المبلغ الإجمالي الصافي لا يمكن أن يكون سالبًا.")
            .When(x => x.SaleItems != null && x.SaleItems.Any());

        // Business rule: Minimum sale amount after discount
        RuleFor(x => x)
            .Must(x => CalculateNetTotal(x.SaleItems, x.Discount) >= MIN_SALE_AMOUNT)
            .WithMessage($"الحد الأدنى لمبلغ البيع هو {MIN_SALE_AMOUNT:C}.")
            .When(x => x.SaleItems != null && x.SaleItems.Any());

        // OPTIONAL: Business rule - Paid amount cannot exceed net total (to prevent overpayment)
        RuleFor(x => x)
            .Must(x => x.PaidAmount <= CalculateNetTotal(x.SaleItems, x.Discount))
            .WithMessage("المبلغ المدفوع يتجاوز المبلغ الإجمالي المستحق.")
            .When(x => x.SaleItems != null && x.SaleItems.Any());
    }

    private bool HaveValidCurrencyPrecision(decimal amount)
    {
        // Get the number of decimal places
        var bits = decimal.GetBits(amount);
        int scale = (bits[3] >> 16) & 0x7F;
        return scale <= MAX_DECIMAL_PLACES;
    }

    private bool BeValidCurrencyAmount(decimal amount)
    {
        // Check if divisible by 0.01 (valid currency amount)
        return amount % 0.01m == 0;
    }

    private bool HaveUniqueProductIds(List<SaleItemRequest> saleItems)
    {
        if (saleItems == null) return true;
        var productIds = saleItems.Select(x => x.ProductID).ToList();
        return productIds.Distinct().Count() == productIds.Count;
    }

    private bool HaveReasonableItemCount(List<SaleItemRequest> saleItems)
    {
        const int MAX_ITEMS_PER_SALE = 100;
        return saleItems?.Count <= MAX_ITEMS_PER_SALE;
    }

    private decimal CalculateSubtotal(List<SaleItemRequest> saleItems)
    {
        if (saleItems == null || !saleItems.Any())
            return 0;

        return saleItems.Sum(item => (decimal)item.Quantity * item.UnitPrice);
    }

    private decimal CalculateNetTotal(List<SaleItemRequest> saleItems, decimal discountAmount)
    {
        var subtotal = CalculateSubtotal(saleItems);
        return subtotal - discountAmount;
    }
}