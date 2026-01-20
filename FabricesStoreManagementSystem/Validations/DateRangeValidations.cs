namespace FabricesStoreManagementSystem.Validations;

public class DateRangeValidations : AbstractValidator<DateRangeRequest>
{
    public DateRangeValidations()
    {

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        RuleFor(x => x.From)
            .LessThanOrEqualTo(today)
            .WithMessage("'Start' date cannot be in the future.")
            .When(x => x.From != default);

        RuleFor(x => x.To)
                    .GreaterThanOrEqualTo(x => x.From)
                    .WithMessage("'End' date must be greater than or equal to From date.")
                    .LessThanOrEqualTo(today)
                    .WithMessage("'End' date cannot be in the future.")
                    .When(x => x.To != default && x.From != default);
    }
}
