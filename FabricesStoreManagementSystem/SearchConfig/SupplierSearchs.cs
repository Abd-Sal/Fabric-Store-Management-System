namespace FabricesStoreManagementSystem.SearchConfig;

public class SupplierSearchs
{
    public static Expression<Func<Supplier, object>> SupplierResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "name" => supplier => supplier.Name,
            "address" => supplier => supplier.Address ?? "",
            "email" => supplier => supplier.Email ?? "",
            "phone" => supplier => supplier.Phone ?? "",
            "id" => supplier => supplier.Id,
            _ => supplier => supplier.Name
        };

    public static SearchColumnsResponse SupplierSortColumns()
    => new SearchColumnsResponse(
            ["name", "address", "email", "phone", "id"],
            "name"
        );
}
