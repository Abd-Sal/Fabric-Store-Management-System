namespace FabricesStoreManagementSystem.Validations;

public class PaginationValidations : AbstractValidator<PaginationRequest>
{
    private const int MIN_PAGE = 1;
    private const int MAX_PAGE = 1000; // Reasonable maximum
    private const int MIN_PAGE_SIZE = 1;
    private const int MAX_PAGE_SIZE = 100;
    private const int DEFAULT_PAGE_SIZE = 10;

    public PaginationValidations()
    {
        // Page validation
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(MIN_PAGE)
            .WithMessage($"رقم الصفحة يجب أن يكون {MIN_PAGE} على الأقل.")
            .LessThanOrEqualTo(MAX_PAGE)
            .WithMessage($"رقم الصفحة لا يمكن أن يتجاوز {MAX_PAGE}.")
            .Must(BeValidPageNumber)
            .WithMessage("رقم الصفحة يجب أن يكون رقمًا صحيحًا موجبًا.");

        // PageSize validation
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(MIN_PAGE_SIZE)
            .WithMessage($"حجم الصفحة يجب أن يكون {MIN_PAGE_SIZE} على الأقل.")
            .LessThanOrEqualTo(MAX_PAGE_SIZE)
            .WithMessage($"حجم الصفحة لا يمكن أن يتجاوز {MAX_PAGE_SIZE}.")
            .Must(BeValidPageSize)
            .WithMessage("حجم الصفحة يجب أن يكون رقمًا صحيحًا موجبًا.");
    }

    private bool BeValidPageNumber(int page)
    {
        return page >= MIN_PAGE && page <= MAX_PAGE;
    }

    private bool BeValidPageSize(int pageSize)
    {
        return pageSize >= MIN_PAGE_SIZE && pageSize <= MAX_PAGE_SIZE;
    }
}