namespace FabricesStoreManagementSystem.Errors;

public class ExpenseErrors
{
    public static Error NotFound =
        new("Expense.NotFound",
            "expense not found!",
            StatusCodes.Status404NotFound);

    public static Error ExpenseIsVeryOld =
        new("Expense.OldExpense",
            "cannot remove expense because it is old!",
            StatusCodes.Status400BadRequest);
}