namespace FabricesStoreManagementSystem.Validations;

public class ProductValidations : AbstractValidator<ProductRequest>
{
    public ProductValidations()
    {
        RuleFor(x => x.Name)
            .Length(1, ProductConfigurations.NameMaxLength)
            .When(x => x.Name is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Material)
            .Length(1, ProductConfigurations.MaterialMaxLength)
            .When(x => x.Material is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Unit)
            .Length(1, ProductConfigurations.UnitMaxLength)
            .When(x => x.Unit is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Code)
            .Length(1, ProductConfigurations.CodeMaxLength)
            .When(x => x.Code is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Color)
            .Length(1, ProductConfigurations.ColorMaxLength)
            .When(x => x.Color is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
    }
}
