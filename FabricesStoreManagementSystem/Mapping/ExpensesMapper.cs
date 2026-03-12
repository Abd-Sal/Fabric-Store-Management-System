namespace FabricesStoreManagementSystem.Mapping;

public static class ExpensesMapper
{
    public static ExpenseResponse ToExpenseResponse(this Expense expense)
        => new ExpenseResponse(expense.Id, expense.Message,
            expense.DollarPriceInSyr, expense.SyrianAmount,
            expense.DollarAmount, expense.CreatedAt);
}