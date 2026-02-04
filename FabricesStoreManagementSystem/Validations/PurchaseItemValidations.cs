namespace FabricesStoreManagementSystem.Validations;

public class PurchaseItemValidations : AbstractValidator<PurchaseItemRequest>
{
    private const float MAX_QUANTITY = 10000f;
    private const decimal MAX_UNIT_COST = 1000000m;
    private const int UNIT_COST_PRECISION = 2; // 2 decimal places for currency

    public PurchaseItemValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Product ID validation
        RuleFor(x => x.ProductID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("معرف المنتج مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.");

        // Unit Cost validation
        RuleFor(x => x.UnitCost)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("تكلفة الوحدة مطلوبة.")
            .GreaterThan(0)
            .WithMessage("تكلفة الوحدة يجب أن تكون أكبر من الصفر.")
            .LessThanOrEqualTo(MAX_UNIT_COST)
            .WithMessage($"تكلفة الوحدة لا يمكن أن تتجاوز {MAX_UNIT_COST:C}.")
            .Must(HaveValidCurrencyPrecision)
            .WithMessage($"تكلفة الوحدة يمكن أن تحتوي على حد أقصى {UNIT_COST_PRECISION} منازل عشرية.")
            .Must(BeValidCurrencyAmount)
            .WithMessage("تكلفة الوحدة يجب أن تكون مضاعفًا للـ 0.01.");

        // Quantity validation (float)
        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("الكمية مطلوبة.")
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من الصفر.")
            .LessThanOrEqualTo(MAX_QUANTITY)
            .WithMessage($"الكمية لا يمكن أن تتجاوز {MAX_QUANTITY}.")
            .Must(BeValidFloatNumber)
            .WithMessage("الكمية يجب أن تكون رقمًا صالحًا.")
            .Must(HasMaximumOneDecimalPlace)
            .WithMessage("الكمية يمكن أن تحتوي على حد أقصى منزلة عشرية واحدة.")
            .Must(BeInTenthIncrements)
            .WithMessage("الكمية يجب أن تكون مضاعفًا للـ 0.1.");
    }

    private bool HaveValidCurrencyPrecision(decimal amount)
    {
        var bits = decimal.GetBits(amount);
        int scale = (bits[3] >> 16) & 0x7F;
        return scale <= UNIT_COST_PRECISION;
    }

    private bool BeValidCurrencyAmount(decimal amount)
    {
        return amount % 0.01m == 0;
    }

    private bool BeValidFloatNumber(float quantity)
    {
        return !float.IsNaN(quantity) && !float.IsInfinity(quantity);
    }

    private bool HasMaximumOneDecimalPlace(float quantity)
    {
        if (float.IsNaN(quantity) || float.IsInfinity(quantity))
            return false;

        try
        {
            // Convert to decimal for precise checking
            decimal decimalQuantity = (decimal)quantity;
            var bits = decimal.GetBits(decimalQuantity);
            int scale = (bits[3] >> 16) & 0x7F;
            return scale <= 1;
        }
        catch (OverflowException)
        {
            // Fallback for very large/small floats
            return ValidateFloatOneDecimalFallback(quantity);
        }
    }

    private bool ValidateFloatOneDecimalFallback(float quantity)
    {
        var str = Math.Abs(quantity).ToString("F10", CultureInfo.InvariantCulture);
        var parts = str.Split('.');

        if (parts.Length == 2)
        {
            var decimalPart = parts[1].TrimEnd('0');
            return decimalPart.Length <= 1;
        }
        return true;
    }

    private bool BeInTenthIncrements(float quantity)
    {
        if (float.IsNaN(quantity) || float.IsInfinity(quantity))
            return false;

        // Multiply by 10 and round to nearest integer
        var scaled = Math.Round(quantity * 10f);
        var difference = Math.Abs(scaled / 10f - quantity);

        const float TOLERANCE = 0.0001f;
        return difference < TOLERANCE;
    }
}