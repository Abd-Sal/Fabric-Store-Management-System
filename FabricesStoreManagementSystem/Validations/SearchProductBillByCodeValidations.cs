namespace FabricesStoreManagementSystem.Validations;

public class SearchProductBillByCodeValidations : AbstractValidator<SearchProductBillByCodeRequest>
{
    public SearchProductBillByCodeValidations()
    {
        RuleFor(x => x.code)
            .Cascade(CascadeMode.Stop)
            .NotNull()
                .WithMessage("رمز المنتج لا يمكن أن يكون فارغاً")
            .Must(code => !string.IsNullOrEmpty(code))
                .WithMessage("رمز المنتج مطلوب")
            .Must(code => !string.IsNullOrWhiteSpace(code))
                .WithMessage("رمز المنتج لا يمكن أن يكون مسافات فقط")
            .Must(code => code.Trim().Length >= 2)
                .WithMessage("رمز المنتج يجب أن يكون على الأقل حرفين")
            .MaximumLength(50)
                .WithMessage("رمز المنتج يجب أن لا يتجاوز 50 حرف")
            .Matches(@"^[\u0600-\u06FFa-zA-Z0-9\-_\s]+$")
                .WithMessage("رمز المنتج يجب أن يحتوي فقط على أحرف عربية وإنجليزية وأرقام وشرطات (- و _) ومسافات");
    }
}