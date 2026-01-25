namespace FabricesStoreManagementSystem.SearchConfig;

public class PurchaseSearchs
{
    public static Expression<Func<Purchase, object>> PurchaseResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => purchase => purchase.Status,
            "invoicenumber" => purchase => purchase.InvoiceNumber,
            "supplierid" => purchase => purchase.SupplierID,
            _ => purchase => purchase.InvoiceNumber
        };

    public static SearchColumnsResponse PurchaseSortColumns()
    => new SearchColumnsResponse(
            ["status", "invoicenumber", "supplierid"],
            "invoicenumber"
        );
}
