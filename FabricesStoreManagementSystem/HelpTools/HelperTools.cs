namespace FabricesStoreManagementSystem.HelpTools;

public class HelperTools
{
    public static string GenerateInvoiceNumber()
        => DateTime.Now.ToString("yyyyMMddHHmmss");

    public static IQueryable<T> HandleStatus<T>(IQueryable<T> query, SearchRequest searchRequest) where T : class, IHasStatus
    {
        var convertToEnum = Enum.TryParse<PayStatuses>(searchRequest.Search, true, out var result);
        if (convertToEnum)
            query = query.Where(x => x.Status == result);
        else if(searchRequest.Search == "unpaid-notcompleted")
            query = query.Where(x => x.Status != PayStatuses.Paid);
        return query;
    }
}
