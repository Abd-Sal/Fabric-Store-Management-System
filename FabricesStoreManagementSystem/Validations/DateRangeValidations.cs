namespace FabricesStoreManagementSystem.Validations;

public class DateRangeValidations : AbstractValidator<DateRangeRequest>
{
    public DateRangeValidations()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("تاريخ البداية مطلوب.")
            .Must(date => date != default)
            .WithMessage("تاريخ البداية لا يمكن أن يكون فارغًا.")
            .LessThanOrEqualTo(today)
            .WithMessage("تاريخ البداية لا يمكن أن يكون في المستقبل.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("تاريخ النهاية مطلوب.")
            .Must(date => date != default)
            .WithMessage("تاريخ النهاية لا يمكن أن يكون فارغًا.")
            .LessThanOrEqualTo(today)
            .WithMessage("تاريخ النهاية لا يمكن أن يكون في المستقبل.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("تاريخ النهاية يجب أن يكون في نفس يوم أو بعد تاريخ البداية.")
            .When(x => x.From != default);

        // Additional business rule: maximum range (e.g., 1 year)
        RuleFor(x => x.To)
            .Must((request, to) => to.DayNumber - request.From.DayNumber <= 365)
            .WithMessage("نطاق التاريخ لا يمكن أن يتجاوز سنة واحدة.")
            .When(x => x.From != default && x.To != default);
    }
}