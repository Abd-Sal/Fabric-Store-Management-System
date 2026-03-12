namespace FabricesStoreManagementSystem.Validations;

public class ExpenseValidations : AbstractValidator<ExpenseRequest>
{

    public ExpenseValidations()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("وصف المصروف مطلوب.")
            .MinimumLength(3)
            .WithMessage("وصف المصروف يجب ألا يقل عن 3 أحرف.")
            .MaximumLength(ExpenseConfigurations.MessageMaxLength)
            .WithMessage("وصف المصروف يجب ألا يتجاوز 100 حرف.");

        RuleFor(x => x.DollarPriceInSyr)
            .GreaterThan(0)
            .WithMessage("سعر الدولار بالليرة السورية يجب أن يكون أكبر من صفر.")
            .PrecisionScale(18, 3, true)
            .WithMessage("سعر الدولار يجب ألا يحتوي على أكثر من 3 أرقام بعد الفاصلة.");

        RuleFor(x => x.SyrianAmount)
            .GreaterThan(0)
            .WithMessage("المبلغ بالليرة السورية يجب أن يكون أكبر من صفر.")
            .PrecisionScale(18, 3, true)
            .WithMessage("المبلغ يجب ألا يحتوي على أكثر من 3 أرقام بعد الفاصلة.");
    }
}