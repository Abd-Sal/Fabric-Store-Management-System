namespace FabricesStoreManagementSystem.SortingConfig;

public static class AssignCatalogSorts
{
    public static Expression<Func<CatalogAssign, object>> AssignCatalogResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "id" => ac => ac.Id,
            "customername" => ac => ac.Customer.FirstName + " " + ac.Customer.LastName,
            "catalogcode" => ac => ac.Catalog.CatalogCode,
            "assignat" => ac => ac.AssignedAt,
            _ => ac => ac.AssignedAt
        };

    public static SortColumnsResponse AssignCatalogSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("اسم العميل", "customername"), new LabelValue("الكود", "catalogcode"),
                new LabelValue("تاريخ الاعارة", "assignat"), new LabelValue("المعرف", "id")],
                new LabelValue("تاريخ الاعارة", "assignat")
            );
}