namespace FabricesStoreManagementSystem.Validations;

public class SaleItemValidations : AbstractValidator<SaleItemRequest>
{
    private const float MAX_QUANTITY = 10000f;
    private const decimal MAX_UNIT_PRICE = 1000000m;
    private const int UNIT_PRICE_PRECISION = 2;

    public SaleItemValidations()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty()
            .WithMessage("معرف المنتج مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.");

        // Quantity is FLOAT - maximum 1 decimal place
        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("الكمية مطلوبة.")
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من الصفر.")
            .LessThanOrEqualTo(MAX_QUANTITY)
            .WithMessage($"الكمية لا يمكن أن تتجاوز {MAX_QUANTITY}.")
            .Must(HaveValidFloatPrecision)  // Max 1 decimal place
            .WithMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى (مثل 1.5، 2.0).")
            .Must(BeReasonableFloatIncrement)  // Increments of 0.1
            .WithMessage("الكمية يجب أن تكون مضاعفًا للـ 0.1 (مثل 0.1، 0.5، 1.0).");

        // UnitPrice is DECIMAL - unchanged (2 decimal places for currency)
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

    // FLOAT version - maximum 1 decimal place
    private bool HaveValidFloatPrecision(float quantity)
    {
        if (float.IsNaN(quantity) || float.IsInfinity(quantity))
            return false;

        // Convert to string and check decimal places
        var str = Math.Abs(quantity).ToString("F10", CultureInfo.InvariantCulture);
        var parts = str.Split('.');

        if (parts.Length == 2)
        {
            var decimalPart = parts[1].TrimEnd('0');
            return decimalPart.Length <= 1; // Only 1 decimal place allowed
        }
        return true;
    }

    // FLOAT version - increments of 0.1 (one decimal place)
    private bool BeReasonableFloatIncrement(float quantity)
    {
        if (float.IsNaN(quantity) || float.IsInfinity(quantity))
            return false;

        // Multiply by 10 and check if integer (for 0.1 increments)
        var multiplied = quantity * 10f;
        var integerPart = Math.Round(multiplied);
        return Math.Abs(multiplied - integerPart) < 0.0001f;
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