namespace FabricesStoreManagementSystem.SearchConfig;

public static class SaleSearchs
{
    public static IQueryable<Sale> SaleResponseSearch(this IQueryable<Sale> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => query.Where(x => EF.Functions.Like(x.Status.ToString(), $"%{searchRequest.Search}%")),
            "invoicenumber" => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
            "customerid" => query.Where(x => EF.Functions.Like(x.CustomerID.ToString(), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
        };

    public static SearchColumnsResponse SaleSortColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("الحالة", "status"), new LabelValue("رقم الفاتورة", "invoicenumber"),
                new LabelValue("معرف الزبون", "customerid")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("رقم الفاتورة", "invoicenumber")
        );
}