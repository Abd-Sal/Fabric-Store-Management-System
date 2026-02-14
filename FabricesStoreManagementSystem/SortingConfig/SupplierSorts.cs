namespace FabricesStoreManagementSystem.SortingConfig;

public class SupplierSorts
{
    public static Expression<Func<Supplier, object>> SupplierResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "name" => supplier => supplier.Name,
            "address" => supplier => supplier.Address ?? "",
            "email" => supplier => supplier.Email ?? "",
            "phone" => supplier => supplier.Phone ?? "",
            "isactive" => supplier => supplier.IsActive,
            "createdat" => supplier => supplier.CreatedAt,
            "id" => supplier => supplier.Id,
            _ => supplier => supplier.CreatedAt
        };

    public static SortColumnsResponse SupplierSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("الاسم", "name"), new LabelValue("العنوان", "address"),
                new LabelValue("البريد الالكتروني", "email"), new LabelValue("رقم الهاتف", "phone"),
                new LabelValue("حالة المورد", "isactive"), new LabelValue("تاريخ الانشاء", "createdat"),
                new LabelValue("المعرف", "id")],
                new LabelValue("تاريخ الانشاء", "createdat")
            );
}