namespace FabricesStoreManagementSystem.SearchConfig;

public static class SaleSearchs
{
    public static IQueryable<Sale> SaleResponseSearch(this IQueryable<Sale> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "invoicenumber" => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
            "customerid" => query.Where(x => EF.Functions.Like(x.CustomerID.ToString(), $"%{searchRequest.Search}%")),
            "customername" => query.Where(x => EF.Functions.Like(x.Customer.FirstName + " " + x.Customer.LastName, $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
        };

    public static SearchColumnsResponse SaleSortColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("رقم الفاتورة", "invoicenumber"),
                new LabelValue("معرف الزبون", "customerid"),
                new LabelValue("اسم الزبون", "customername")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("رقم الفاتورة", "invoicenumber")
        );
}