namespace FabricesStoreManagementSystem.Validations;

public class DateRangeValidations : AbstractValidator<DateRangeRequest>
{
    public DateRangeValidations()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Either both From & To are provided, or neither
        RuleFor(x => x)
            .Must(x =>
                (x.From.HasValue && x.To.HasValue) ||
                (!x.From.HasValue && !x.To.HasValue))
            .WithMessage("يجب إدخال تاريخ البداية وتاريخ النهاية معًا أو تركهما فارغين.");

        // From validations (only if provided)
        RuleFor(x => x.From)
            .LessThanOrEqualTo(today)
            .WithMessage("تاريخ البداية لا يمكن أن يكون في المستقبل.")
            .When(x => x.From.HasValue);

        // To validations (only if provided)
        RuleFor(x => x.To)
            .LessThanOrEqualTo(today)
            .WithMessage("تاريخ النهاية لا يمكن أن يكون في المستقبل.")
            .When(x => x.To.HasValue);

        // From <= To (only if both provided)
        RuleFor(x => x)
            .Must(x => x.From <= x.To)
            .WithMessage("تاريخ النهاية يجب أن يكون في نفس يوم أو بعد تاريخ البداية.")
            .When(x => x.From.HasValue && x.To.HasValue);

        // Max range: 1 year (only if both provided)
        RuleFor(x => x)
            .Must(x => x.To!.Value.DayNumber - x.From!.Value.DayNumber <= 365)
            .WithMessage("نطاق التاريخ لا يمكن أن يتجاوز سنة واحدة.")
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}
