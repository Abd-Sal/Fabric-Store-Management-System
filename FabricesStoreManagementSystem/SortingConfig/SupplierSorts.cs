namespace FabricesStoreManagementSystem.SortingConfig;

public class SupplierSorts
{
    public static Expression<Func<Supplier, object>> SupplierResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "name" => supplier => supplier.Name,
            "address" => supplier => supplier.Address ?? "",
            "email" => supplier => supplier.Email != null ? supplier.Email : supplier.CreatedAt,
            "phone" => supplier => supplier.Phone != null ? supplier.Phone : supplier.CreatedAt,
            "isactive" => supplier => supplier.IsActive,
            "createdat" => supplier => supplier.CreatedAt,
            "id" => supplier => supplier.Id,
            _ => supplier => supplier.CreatedAt
        };

    public static SortColumnsResponse SupplierSortColumns()
        => new SortColumnsResponse(
                ["name", "address", "email", "phone", "isactive", "createdat", "id"],
                "createdat"
            );
}