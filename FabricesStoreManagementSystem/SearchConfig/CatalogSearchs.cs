namespace FabricesStoreManagementSystem.SearchConfig;

public static class CatalogSearchs
{
    public static IQueryable<Catalog> CatalogResponseSearch(this IQueryable<Catalog> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => query.Where(x => EF.Functions.Like(x.Status.ToString(), $"%{searchRequest.Search}%")),
            "description" => query.Where(x => EF.Functions.Like(x.Description ?? "", $"%{searchRequest.Search}%")),
            "supplierid" => query.Where(x => EF.Functions.Like(x.SupplierID.ToString(), $"%{searchRequest.Search}%")),
            "code" => query.Where(x => EF.Functions.Like(x.CatalogCode, $"%{searchRequest.Search}%")),
            "id" => query.Where(x => EF.Functions.Like(x.Id.ToString(), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.CatalogCode, $"%{searchRequest.Search}%")),
        };

    public static SearchColumnsResponse CatalogSortColumns()
        => new SearchColumnsResponse(
                new List<LabelValue>{
                    new LabelValue("الوصف", "description"), new LabelValue("الحالة", "status"),
                    new LabelValue("الكود", "code"), new LabelValue("معرف المورد", "supplierid"),
                    new LabelValue("المعرف", "id")
                }.OrderBy(x => x.Label).ToArray(),
                new LabelValue("الكود", "code")
            );
}
