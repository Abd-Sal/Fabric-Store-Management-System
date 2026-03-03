namespace FabricesStoreManagementSystem.Validations;

public class ProductCatalogValidations : AbstractValidator<ProductCatalogRequest>
{
    private const decimal MIN_QUANTITY = 0.1m;    // Minimum 0.1 units
    private const decimal MAX_QUANTITY = 20m;     // Maximum 20 units (from your rule)

    public ProductCatalogValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Product ID validation
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("معرف المنتج مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.")
            .NotEmpty()
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.");

        // Quantity validation (decimal)
        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("الكمية مطلوبة.")
            .GreaterThan(0)
            .WithMessage("الكمية يجب أن تكون أكبر من الصفر.")
            .GreaterThanOrEqualTo(MIN_QUANTITY)
            .WithMessage($"الكمية يجب أن تكون {MIN_QUANTITY} على الأقل.")
            .LessThanOrEqualTo(MAX_QUANTITY)
            .WithMessage($"الكمية لا يمكن أن تتجاوز {MAX_QUANTITY}.")
            .Must(HasMaximumTwoDecimalPlaces)
            .WithMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.")
            .Must(BeInHundredthIncrements)
            .WithMessage("الكمية يجب أن تكون مضاعفًا للـ 0.01 (مثل 0.01، 1.25، 2.50).");
    }


    private bool HasMaximumTwoDecimalPlaces(decimal quantity)
    {
        try
        {
            var bits = decimal.GetBits(quantity);
            int scale = (bits[3] >> 16) & 0x7F; // Get decimal places
            return scale <= 2;
        }
        catch (OverflowException)
        {
            // Fallback for very large/small decimals
            return ValidatedecimalOneDecimalFallback(quantity);
        }
    }

    private bool ValidatedecimalOneDecimalFallback(decimal quantity) // fallback still OK for 2 digits
    {
        // String-based validation for decimals
        var str = Math.Abs(quantity).ToString("F10", CultureInfo.InvariantCulture);
        var parts = str.Split('.');

        if (parts.Length == 2)
        {
            var decimalPart = parts[1].TrimEnd('0');
            return decimalPart.Length <= 1;
        }
        return true; // No decimal point
    }

    private bool BeInHundredthIncrements(decimal quantity)
    {
        // Exact for `decimal`: multiples of 0.01 have (quantity * 100) as an integer.
        var scaled = quantity * 100m;
        return (scaled % 1m) == 0m;
    }
}