namespace FabricesStoreManagementSystem.SearchConfig;

public static class ExpenseSearchs
{
    public static IQueryable<Expense> ExpenseResponseSearch(this IQueryable<Expense> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "id" => query.Where(x => EF.Functions.Like(x.Id.ToString(), $"%{searchRequest.Search}%")),
            "message" => query.Where(x => EF.Functions.Like(x.Message, $"%{searchRequest.Search}%")),
            "syrianamount" => query.Where(x => EF.Functions.Like(x.SyrianAmount.ToString(), $"%{searchRequest.Search}%")),
            "dollarpriceinsyr" => query.Where(x => EF.Functions.Like(x.DollarPriceInSyr.ToString(), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.Message, $"%{searchRequest.Search}%"))
        };

    public static SearchColumnsResponse ExpenseSearchColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("سعر الدولار", "dollarpriceinsyr"), new LabelValue("القيمة بالليرة السورية", "syrianamount"),
                new LabelValue("المعرف", "id"), new LabelValue("الوصف", "message")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("الوصف", "message")
        );
}