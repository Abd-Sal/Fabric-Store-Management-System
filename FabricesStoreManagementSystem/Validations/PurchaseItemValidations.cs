namespace FabricesStoreManagementSystem.Validations;

public class PurchaseItemValidations : AbstractValidator<PurchaseItemRequest>
{
    public PurchaseItemValidations()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty();

        RuleFor(x => x.UnitCost)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);
    }
}
