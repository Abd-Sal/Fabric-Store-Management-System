namespace FabricesStoreManagementSystem.Validations;

public class CustomerSearchForBillValidations : AbstractValidator<CustomerSearchForBillRequest>
{
    public CustomerSearchForBillValidations()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithMessage("الاسم لا يمكن أن يكون فارغاً")
            .Must(name => !string.IsNullOrEmpty(name))
                .WithMessage("الاسم مطلوب")
            .Must(code => !string.IsNullOrWhiteSpace(code))
                .WithMessage("الاسم لا يمكن أن يكون مسافات فقط")
            .Must(code => code.Trim().Length >= 2)
                .WithMessage("الاسم يجب أن يكون على الأقل حرفين")
            .MaximumLength(50)
                .WithMessage("الاسم يجب أن لا يتجاوز 50 حرف")
            .Matches(@"^[\u0600-\u06FFa-zA-Z0-9\-_\s]+$")
                .WithMessage("الاسم يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }
}