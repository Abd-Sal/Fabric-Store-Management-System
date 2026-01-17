namespace FabricesStoreManagementSystem.Validations;

public class AuthValidations : AbstractValidator<LoginRequest>
{
    public AuthValidations()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(1, 100)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .Length(1, 100)
            .WithMessage("'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters.");
    }
}
