namespace FabricesStoreManagementSystem.SortingConfig;

public class CustomerSorts
{
    public static Expression<Func<Customer, object>> CustomerResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "name" => customer => $"{customer.FirstName} {customer.LastName}",
            "address" => customer => customer.Address ?? "",
            "email" => customer => customer.Email ?? "",
            "phone" => customer => customer.Phone ?? "",
            "isactive" => customer => customer.IsActive,
            "createdat" => customer => customer.CreatedAt,
            "id" => customer => customer.Id,
            _ => customer => customer.CreatedAt
        };

    public static SortColumnsResponse CustomerSortColumns()
    => new SortColumnsResponse(
            [new LabelValue("الاسم", "name"), new LabelValue("العنوان", "address"),
            new LabelValue("البريد الالكتروني", "email"), new LabelValue("رقم الهاتف", "phone"),
            new LabelValue("حالة الزبون", "isactive"), new LabelValue("تاريخ الانشاء", "createdat"),
            new LabelValue("المعرف", "id")],
            new LabelValue("تاريخ الانشاء", "createdat")
        );
}
