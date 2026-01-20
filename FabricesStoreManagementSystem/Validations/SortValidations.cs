namespace FabricesStoreManagementSystem.Validations;

public class SortValidations : AbstractValidator<SortRequest>
{
    public SortValidations()
    {
        RuleFor(x => x.SortDir)
            .Must(x => new[] { "asc", "desc" }.Contains(x?.ToLower()))
            .When(x => !string.IsNullOrWhiteSpace(x.SortDir))
            .WithMessage("SortDir must be either 'asc' or 'desc'.");

        RuleFor(x => x.SortColumn)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .When(x => !string.IsNullOrEmpty(x.SortColumn))
            .WithMessage("SortColumn can only contain letters, numbers, and underscores, and must start with a letter or underscore."); ;

        RuleFor(x => x.SortColumn)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.SortColumn))
            .WithMessage("SortColumn cannot exceed 50 characters.");
    }
}