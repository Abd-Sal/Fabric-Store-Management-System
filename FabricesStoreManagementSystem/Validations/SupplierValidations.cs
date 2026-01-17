namespace FabricesStoreManagementSystem.Validations;

public class SupplierValidations : AbstractValidator<SupplierRequest>
{
    public SupplierValidations()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(1, SupplierConfigurations.NameMaxLength)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Email)
            .Length(1, SupplierConfigurations.EmailMaxLength)
            .When(x => x.Email is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Phone)
            .Length(1, SupplierConfigurations.PhoneMaxLength)
            .When(x => x.Email is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Address)
            .Length(1, SupplierConfigurations.AddressMaxLength)
            .When(x => x.Address is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
    }
}
