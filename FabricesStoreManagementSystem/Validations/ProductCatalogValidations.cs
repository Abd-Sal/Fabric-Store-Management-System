namespace FabricesStoreManagementSystem.Validations;

public class ProductCatalogValidations : AbstractValidator<ProductCatalogRequest>
{
    private const float MIN_QUANTITY = 0.1f;    // Minimum 0.1 units
    private const float MAX_QUANTITY = 20f;     // Maximum 20 units (from your rule)
    private const float TOLERANCE = 0.00001f;   // For float comparisons

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

        // Quantity validation (float)
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
            .Must(BeValidFloatNumber)
            .WithMessage("الكمية يجب أن تكون رقمًا صالحًا.")
            .Must(HasMaximumOneDecimalPlace)
            .WithMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى.")
            .Must(BeInTenthIncrements)
            .WithMessage("الكمية يجب أن تكون مضاعفًا للـ 0.1 (مثل 0.2، 1.0، 2.5).");
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
            // Convert to decimal for precise decimal place checking
            decimal decimalQuantity = (decimal)quantity;
            var bits = decimal.GetBits(decimalQuantity);
            int scale = (bits[3] >> 16) & 0x7F; // Get decimal places
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
        // String-based validation for floats
        var str = Math.Abs(quantity).ToString("F10", CultureInfo.InvariantCulture);
        var parts = str.Split('.');

        if (parts.Length == 2)
        {
            var decimalPart = parts[1].TrimEnd('0');
            return decimalPart.Length <= 1;
        }
        return true; // No decimal point
    }

    private bool BeInTenthIncrements(float quantity)
    {
        if (float.IsNaN(quantity) || float.IsInfinity(quantity))
            return false;

        // Scale by 10 and allow tiny floating-point tolerance
        var scaled = quantity * 10f;

        return Math.Abs(scaled - MathF.Round(scaled)) < TOLERANCE;
    }
}