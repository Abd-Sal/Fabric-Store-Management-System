namespace FabricesStoreManagementSystem.SearchConfig;

public class SaleSearchs
{
    public static Expression<Func<Sale, object>> SaleResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => sale => sale.Status,
            "invoicenumber" => sale => sale.InvoiceNumber,
            "customerid" => sale => sale.CustomerID,
            _ => product => product.InvoiceNumber
        };

    public static SearchColumnsResponse SaleSortColumns()
    => new SearchColumnsResponse(
            [new LabelValue("الحالة", "status"), new LabelValue("رقم الفاتورة", "invoicenumber"),
            new LabelValue("معرف الزبون", "customerid")],
            new LabelValue("رقم الفاتورة", "invoicenumber")
        );
}