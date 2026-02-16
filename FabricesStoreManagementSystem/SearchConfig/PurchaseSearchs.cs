namespace FabricesStoreManagementSystem.SearchConfig;

public static class PurchaseSearchs
{
    public static IQueryable<Purchase> PurchaseResponseSearch(this IQueryable<Purchase> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => query.Where(x => EF.Functions.Like(x.Status.ToString(), $"%{searchRequest.Search}%")),
            "invoicenumber" => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
            "supplierid" => query.Where(x => EF.Functions.Like(x.SupplierID.ToString(), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
        };

    public static SearchColumnsResponse PurchaseSortColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("الحالة", "status"), new LabelValue("رقم الفاتورة", "invoicenumber"),
                new LabelValue("معرف المورد", "supplierid")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("رقم الفاتورة", "invoicenumber")
        );
}
