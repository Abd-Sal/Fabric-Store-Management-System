namespace FabricesStoreManagementSystem.Validations;

public class ProductCodeValidations : AbstractValidator<ProductCodeRequest>
{
    public ProductCodeValidations()
    {
        RuleFor(x => x.code)
                    .NotEmpty().WithMessage("رمز المنتج مطلوب")
                    .NotNull().WithMessage("رمز المنتج مطلوب")
                    .MinimumLength(1).WithMessage("يجب أن يكون رمز المنتج على الأقل 1 أحرف")
                    .MaximumLength(ProductConfigurations.CodeMaxLength).WithMessage($"يجب أن يكون رمز المنتج على الأكثر {ProductConfigurations.CodeMaxLength} حرف");
    }
}
