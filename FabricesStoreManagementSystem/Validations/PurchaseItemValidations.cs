namespace FabricesStoreManagementSystem.Validations;

public class PurchaseItemValidations : AbstractValidator<PurchaseItemRequest>
{
    private const decimal MAX_QUANTITY = 10000m;
    private const decimal MAX_UNIT_COST = 1000000m;
    private const int UNIT_COST_PRECISION = 3;

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
            .WithMessage("تكلفة الوحدة يجب أن تكون مضاعفًا للـ 0.001.");

        // Quantity validation (decimal)
        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("الكمية مطلوبة.")
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من الصفر.")
            .LessThanOrEqualTo(MAX_QUANTITY)
            .WithMessage($"الكمية لا يمكن أن تتجاوز {MAX_QUANTITY}.")
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
        return amount % 0.001m == 0;
    }

    private bool HasMaximumOneDecimalPlace(decimal quantity)
    {
        try
        {
            // Convert to decimal for precise checking
            var bits = decimal.GetBits(quantity);
            int scale = (bits[3] >> 16) & 0x7F;
            return scale <= 2;
        }
        catch (OverflowException)
        {
            // Fallback for very large/small decimals
            return ValidatedecimalOneDecimalFallback(quantity);
        }
    }

    private bool ValidatedecimalOneDecimalFallback(decimal quantity)
    {
        var str = Math.Abs(quantity).ToString("F10", CultureInfo.InvariantCulture);
        var parts = str.Split('.');

        if (parts.Length == 2)
        {
            var decimalPart = parts[1].TrimEnd('0');
            return decimalPart.Length <= 2;
        }
        return true;
    }

    private bool BeInTenthIncrements(decimal quantity)
    {
        // Exact for `decimal`: multiples of 0.1 have (quantity * 10) as an integer.
        var scaled = quantity * 10m;
        return (scaled % 1m) == 0m;
    }
}