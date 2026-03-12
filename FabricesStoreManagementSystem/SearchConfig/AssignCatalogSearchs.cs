namespace FabricesStoreManagementSystem.SearchConfig;

public static class AssignCatalogSearchs
{
    public static IQueryable<CatalogAssign> AssignCatalogResponseSearch(this IQueryable<CatalogAssign> query, SearchRequest searchRequest)
    => searchRequest.SearchColumn?.ToLower() switch
    {
        "customerid" => query.Where(x => EF.Functions.Like(x.CustomerID.ToString(), $"%{searchRequest.Search}%")),
        "customername" => query.Where(x => EF.Functions.Like(x.Customer.FirstName + " " + x.Customer.LastName, $"%{searchRequest.Search}%")),
        "catalogid" => query.Where(x => EF.Functions.Like(x.CatalogID.ToString(), $"%{searchRequest.Search}%")),
        "catalogcode" => query.Where(x => EF.Functions.Like(x.Catalog.CatalogCode, $"%{searchRequest.Search}%")),
        "id" => query.Where(x => EF.Functions.Like(x.Catalog.CatalogCode, $"%{searchRequest.Search}%")),
        _ => query.Where(x => EF.Functions.Like(x.Catalog.CatalogCode, $"%{searchRequest.Search}%")),
    };

    public static SearchColumnsResponse AssignCatalogSearchColumns()
        => new SearchColumnsResponse(
                new List<LabelValue>{
                    new LabelValue("اسم العميل", "customername"), new LabelValue("الكود", "catalogcode"),
                    new LabelValue("معرف العميل", "customerid"), new LabelValue("معرف الكانالوغ", "cattalogid"),
                    new LabelValue("المعرف", "id")
                }.OrderBy(x => x.Label).ToArray(),
                new LabelValue("الكود", "catalogcode")
            );
}
