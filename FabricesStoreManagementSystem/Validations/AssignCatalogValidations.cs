namespace FabricesStoreManagementSystem.Validations;

public class AssignCatalogValidations : AbstractValidator<AssignCatalogRequest>
{
    public AssignCatalogValidations()
    {
        RuleFor(x => x.CustomerID)
            .NotEmpty().WithMessage("معرف العميل مطلوب.")
            .NotEqual(Guid.Empty).WithMessage("معرف العميل غير صالح.");

        RuleFor(x => x.CatalogID)
            .NotEmpty().WithMessage("معرف الكتالوج مطلوب.")
            .NotEqual(Guid.Empty).WithMessage("معرف الكتالوج غير صالح.");

        // Optional: Prevent self-assignment (if CustomerID and CatalogID could be same type)
        RuleFor(x => x)
            .Must(x => x.CustomerID != x.CatalogID)
            .WithMessage("لا يمكن تعيين كتالوج لنفسه.")
            .When(x => x.CustomerID != Guid.Empty && x.CatalogID != Guid.Empty);
    }
}