namespace FabricesStoreManagementSystem.SortingConfig;

public class ExpenseSorts
{
    public static Expression<Func<Expense, object>> ExpenseResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "message" => expense => expense.Message,
            "createdat" => expense => expense.CreatedAt,
            "syrianamount" => expense => expense.SyrianAmount,
            "dollarpriceinsyr" => expense => expense.DollarPriceInSyr,
            "id" => expense => expense.Id,
            _ => expense => expense.CreatedAt
        };

    public static SortColumnsResponse ExpenseSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("الوصف", "message"), new LabelValue("سعر الدولار", "dollarpriceinsyr"),
                new LabelValue("القيمة بالليرة السورية", "syrianamount"),new LabelValue("تاريخ الدفع", "createdat"),
                new LabelValue("المعرف", "id")],
                new LabelValue("تاريخ الدفع", "createdat")
            );
}