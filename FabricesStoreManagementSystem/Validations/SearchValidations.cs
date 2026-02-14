namespace FabricesStoreManagementSystem.Validations;

public class SearchValidations : AbstractValidator<SearchRequest>
{
    private const int MAX_SEARCH_LENGTH = 100;
    private const int MAX_SEARCH_COLUMN_LENGTH = 50;
    
    public SearchValidations()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;

        // Search term validation
        RuleFor(x => x.Search)
            .MaximumLength(MAX_SEARCH_LENGTH)
            .WithMessage($"مصطلح البحث لا يمكن أن يتجاوز {MAX_SEARCH_LENGTH} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        // Search column validation - validate only when provided
        RuleFor(x => x.SearchColumn)
            .MaximumLength(MAX_SEARCH_COLUMN_LENGTH)
            .WithMessage($"عمود البحث لا يمكن أن يتجاوز {MAX_SEARCH_COLUMN_LENGTH} حرفًا.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchColumn))
            .Matches(@"^[a-zA-Z_][a-zA-Z0-9_]*$")
            .WithMessage("عمود البحث يجب أن يبدأ بحرف إنجليزي أو شرطة سفلية ويحتوي على أحرف إنجليزية وأرقام وشرطات سفلية فقط.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchColumn));

        // Cross-validation: If SearchColumn is provided, Search should be meaningful
        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.SearchColumn) ||
                        (!string.IsNullOrWhiteSpace(x.Search)))
            .WithMessage("عند تحديد عمود البحث، يجب إدخال مصطلح بحث ذو معنى.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchColumn));
    }
}