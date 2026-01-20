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

        RuleFor(x => x.PurchaseItems)
            .NotEmpty()
            .Must(x => x.Count == x.DistinctBy(x => x.ProductID).Count())
            .WithMessage("there is duplicated product id");

        RuleForEach(x => x.PurchaseItems)
            .NotEmpty()
            .SetValidator(new PurchaseItemValidations());
    }
}
