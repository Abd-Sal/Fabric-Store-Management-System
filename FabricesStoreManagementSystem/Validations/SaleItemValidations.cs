namespace FabricesStoreManagementSystem.Validations;

public class SaleItemValidations : AbstractValidator<SaleItemRequest>
{
    public SaleItemValidations()
    {
        RuleFor(x => x.ProductID)
            .NotEmpty();
        
        RuleFor(x => x.Qunatity)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.UnitPrice)
            .NotEmpty()
            .GreaterThan(0);
    }
}
