namespace FabricesStoreManagementSystem.Validations;

public class CatalogProductValidations : AbstractValidator<CatalogProductRequest>
{
    private const float MIN_QUANTITY = 0.1f;
    private const float MAX_QUANTITY = 100f; // Reasonable maximum for catalog items

    public CatalogProductValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Product ID validation
        RuleFor(x => x.ProductID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("معرف المنتج مطلوب.")
            .NotEqual(Guid.Empty)
            .WithMessage("معرف المنتج لا يمكن أن يكون فارغًا.");

        // Quantity validation (float)
        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
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
            .WithMessage("الكمية يمكن أن تحتوي على منزلة عشرية واحدة كحد أقصى.");
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
            decimal decimalQuantity = (decimal)quantity;
            var bits = decimal.GetBits(decimalQuantity);
            int scale = (bits[3] >> 16) & 0x7F; // Get decimal places
            return scale <= 1;
        }
        catch (OverflowException)
        {
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
}