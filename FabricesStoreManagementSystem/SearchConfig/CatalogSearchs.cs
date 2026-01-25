namespace FabricesStoreManagementSystem.SearchConfig;

public class CatalogSearchs
{
    public static Expression<Func<Catalog, object>> CatalogResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "status" => catalog => catalog.Status,
            "description" => catalog => catalog.Description ?? "",
            "supplierid" => catalog => catalog.SupplierID ?? default(Guid),
            "code" => catalog => catalog.CatalogCode,
            "id" => catalog => catalog.Id,
            _ => catalog => catalog.CatalogCode
        };

    public static SearchColumnsResponse CatalogSortColumns()
        => new SearchColumnsResponse(
                ["description", "status", "code", "supplierid", "id"],
                "code"
            );
}
