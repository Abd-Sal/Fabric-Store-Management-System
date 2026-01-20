namespace FabricesStoreManagementSystem.Validations;

public class ProductCatalogValidations : AbstractValidator<ProductCatalogRequest>
{
    public ProductCatalogValidations()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .GreaterThan(0)
            .LessThanOrEqualTo(20);
    }
}