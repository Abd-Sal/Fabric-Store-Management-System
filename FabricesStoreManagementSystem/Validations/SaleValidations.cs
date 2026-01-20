namespace FabricesStoreManagementSystem.Validations;

public class SaleValidations : AbstractValidator<SaleRequest>
{
    public SaleValidations()
    {
        RuleFor(x => x.CustomerID)
            .NotEmpty();

        RuleFor(x => x.Discount)
            .NotEmpty();

        RuleFor(x => x.PaidAmount)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.SaleItems)
            .NotEmpty()
            .Must(x => x.Count == x.DistinctBy(x => x.ProductID).Count())
            .WithMessage("there is duplicated product id");

        RuleForEach(x => x.SaleItems)
            .SetValidator(new SaleItemValidations());
    }
}
