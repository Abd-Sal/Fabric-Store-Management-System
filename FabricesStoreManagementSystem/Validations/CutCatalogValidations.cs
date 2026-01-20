namespace FabricesStoreManagementSystem.Validations;

public class CutCatalogValidations : AbstractValidator<CutCatalogRequest>
{
    public CutCatalogValidations()
    {
        RuleFor(x => x.Product)
            .NotEmpty()
            .Must(x => x.Count == x.DistinctBy(x => x.Id).Count())
            .WithMessage("there is duplicated product id");

        RuleForEach(x => x.Product)
            .SetValidator(new ProductCatalogValidations());
    }
}
