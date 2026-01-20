namespace FabricesStoreManagementSystem.Validations;

public class PaginationValidations : AbstractValidator<PaginationRequest>
{
    public PaginationValidations()
    {
        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
    }
}
