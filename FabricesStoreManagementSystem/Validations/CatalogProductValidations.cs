namespace FabricesStoreManagementSystem.Validations;

public class CatalogProductValidations : AbstractValidator<CatalogProductRequest>
{
    private const decimal MIN_QUANTITY = 0.1m;
    private const decimal MAX_QUANTITY = 100m;

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

        // Quantity validation (decimal)
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
            .Must(HasMaximumTwoDecimalPlace)
            .WithMessage("الكمية يمكن أن تحتوي على منزلتين عشريتين كحد أقصى.");
    }

    private bool HasMaximumTwoDecimalPlace(decimal quantity)
    {
        try
        {
            var bits = decimal.GetBits(quantity);
            int scale = (bits[3] >> 16) & 0x7F; // Get decimal places
            return scale <= 2;
        }
        catch (OverflowException)
        {
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
}