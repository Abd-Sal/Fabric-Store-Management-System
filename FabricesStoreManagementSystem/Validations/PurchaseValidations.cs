namespace FabricesStoreManagementSystem.Validations;

public class PurchaseValidations : AbstractValidator<PurchaseRequest>
{
    public PurchaseValidations()
    {
        RuleFor(x => x.SupplierID)
            .NotEmpty();

        RuleFor(x => x.PaidAmount)
            .NotEmpty()
            .GreaterThanOrEqualTo(0);

        RuleForEach(x => x.PurchaseItems)
            .NotEmpty()
            .SetValidator(new PurchaseItemValidations());
    }
}
