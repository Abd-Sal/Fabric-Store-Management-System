namespace FabricesStoreManagementSystem.Validations;

public class CustomerValidations : AbstractValidator<CustomerRequest>
{
    public CustomerValidations()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(1, CustomerConfigurations.FirstNameMaxLength)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(1, CustomerConfigurations.LastNameMaxLength)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Email)
            .Length(1, CustomerConfigurations.EmailMaxLength)
            .When(x => x.Email is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Phone)
            .Length(1, CustomerConfigurations.PhoneMaxLength)
            .When(x => x.Email is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Address)
            .Length(1, CustomerConfigurations.AddressMaxLength)
            .When(x => x.Address is not null)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
    }
}
