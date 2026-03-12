namespace FabricesStoreManagementSystem.SearchConfig;

public static class PurchaseSearchs
{
    public static IQueryable<Purchase> PurchaseResponseSearch(this IQueryable<Purchase> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "invoicenumber" => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
            "supplierid" => query.Where(x => EF.Functions.Like(x.SupplierID.ToString(), $"%{searchRequest.Search}%")),
            "status" => query.Where(x => EF.Functions.Like(x.Status.ToString(), $"%{searchRequest.Search}%")),
            "suppliername" => query.Where(x => EF.Functions.Like(x.Supplier.Name, $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.InvoiceNumber, $"%{searchRequest.Search}%")),
        };

    public static SearchColumnsResponse PurchaseSearchColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("اسم المورد", "suppliername"),
                new LabelValue("رقم الفاتورة", "invoicenumber"),
                new LabelValue("معرف المورد", "supplierid")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("رقم الفاتورة", "invoicenumber")
        );
}
