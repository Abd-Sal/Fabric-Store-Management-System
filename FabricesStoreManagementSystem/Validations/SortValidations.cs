namespace FabricesStoreManagementSystem.Validations;

public class SortValidations : AbstractValidator<SortRequest>
{
    public SortValidations()
    {
        // Validate SortColumn if provided
        When(x => !string.IsNullOrWhiteSpace(x.SortColumn), () =>
        {
            RuleFor(x => x.SortColumn)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("عمود الفرز لا يمكن أن يكون فارغًا.")
                .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$")
                .WithMessage("عمود الفرز يجب أن يبدأ بحرف إنجليزي أو شرطة سفلية ويحتوي على أحرف إنجليزية وأرقام وشرطات سفلية فقط.")
                .MaximumLength(50)
                .WithMessage("عمود الفرز لا يمكن أن يتجاوز 50 حرفًا.")
                .Must(column => !column.Contains(" "))
                .WithMessage("عمود الفرز لا يمكن أن يحتوي على مسافات.");
        });

        // Validate SortDir if provided
        When(x => !string.IsNullOrWhiteSpace(x.SortDir), () =>
        {
            RuleFor(x => x.SortDir)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("اتجاه الفرز لا يمكن أن يكون فارغًا.")
                .Must(x => BeValidSortDirection(x!))
                .WithMessage("اتجاه الفرز يجب أن يكون 'asc' أو 'desc' (تصاعدي أو تنازلي).");
        });

        // Cross-validation: SortDir requires SortColumn
        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.SortDir) ||
                      !string.IsNullOrWhiteSpace(x.SortColumn))
            .WithMessage("لا يمكن تحديد اتجاه الفرز بدون تحديد عمود الفرز.")
            .WithName("SortConsistency");
    }

    private bool BeValidSortDirection(string sortDir)
    {
        if (string.IsNullOrWhiteSpace(sortDir))
            return true;

        var normalized = sortDir.Trim().ToLowerInvariant();

        var validDirections = new HashSet<string>
        {
            "asc", "desc",
            "ascending", "descending",
        };

        return validDirections.Contains(normalized);
    }
}