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

        RuleForEach(x => x.SaleItems)
            .SetValidator(new SaleItemValidations());
    }
}
