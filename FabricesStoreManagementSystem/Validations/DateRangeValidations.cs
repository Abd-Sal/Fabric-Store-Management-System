namespace FabricesStoreManagementSystem.Validations;

public class DateRangeValidations : AbstractValidator<DateRangeRequest>
{
    public DateRangeValidations()
    {

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("'{PropertyName}' date is required.")
            .Must(date => date != default)
            .WithMessage("'From' date cannot be default.")
            .LessThanOrEqualTo(today)
            .WithMessage("'{PropertyName}' date cannot be in the future.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("'{PropertyName}' date is required.")
            .Must(date => date != default)
            .WithMessage("'{PropertyName}' date cannot be default.")
            .LessThanOrEqualTo(today)
            .WithMessage("'{PropertyName}' date cannot be in the future.")
            .GreaterThanOrEqualTo(x => x.From)
            .WithMessage("'{PropertyName}' date must be on or after 'From' date.")
            .When(x => x.From != default);

        // Additional business rule: maximum range (e.g., 1 year)
        RuleFor(x => x.To)
            .Must((request, to) => to.DayNumber - request.From.DayNumber <= 365)
            .WithMessage("Date range cannot exceed 1 year.")
            .When(x => x.From != default && x.To != default);
    }
}
