namespace FabricesStoreManagementSystem.Validations;

public class SaleItemValidations : AbstractValidator<SaleItemRequest>
{
    private const decimal MAX_QUANTITY = 10000m;
    private const decimal MAX_UNIT_PRICE = 1000000m;
    private const int UNIT_PRICE_PRECISION = 3;

    public SaleItemValidations()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty()
            .WithMessage("معرف المنتج مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.");

        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("الكمية مطلوبة.")
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من الصفر.")
            .LessThanOrEqualTo(MAX_QUANTITY)
            .WithMessage($"الكمية لا يمكن أن تتجاوز {MAX_QUANTITY}.")
            .Must(HaveValiddecimalPrecision) 
            .WithMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.")
            .Must(BeReasonabledecimalIncrement)  // Increments of 0.1
            .WithMessage("الكمية يجب أن تكون مضاعفًا للـ 0.1 (مثل 0.1، 0.5، 1.0).");

        RuleFor(x => x.UnitPrice)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("سعر الوحدة مطلوب.")
            .GreaterThan(0)
            .WithMessage($"سعر الوحدة يجب أن يكون أكبر من {0:C}.")
            .LessThanOrEqualTo(MAX_UNIT_PRICE)
            .WithMessage($"سعر الوحدة لا يمكن أن يتجاوز {MAX_UNIT_PRICE:C}.")
            .Must(HaveValidCurrencyPrecision)
            .WithMessage($"سعر الوحدة يمكن أن يحتوي على حد أقصى {UNIT_PRICE_PRECISION} منازل عشرية.")
            .Must(BeValidPricePoint)
            .WithMessage("سعر الوحدة يجب أن يكون مبلغًا نقديًا صالحًا.");
    }

    private bool HaveValiddecimalPrecision(decimal quantity)
    {
        var bits = decimal.GetBits(quantity);
        int scale = (bits[3] >> 16) & 0x7F;
        return scale <= 2;
    }

    // decimal version - increments of 0.1 (one decimal place)
    private bool BeReasonabledecimalIncrement(decimal quantity)
    {
        // Exact for `decimal`: multiples of 0.1 have (quantity * 10) as an integer.
        var scaled = quantity * 10m;
        return (scaled % 1m) == 0m;
    }

    // DECIMAL version (unchanged)
    private bool HaveValidCurrencyPrecision(decimal price)
    {
        var decimalPlaces = BitConverter.GetBytes(decimal.GetBits(price)[3])[2];
        return decimalPlaces <= UNIT_PRICE_PRECISION;
    }

    // DECIMAL version (unchanged)
    private bool BeValidPricePoint(decimal price)
    {
        var remainder = price % 0.01m;
        return remainder == 0;
    }
}