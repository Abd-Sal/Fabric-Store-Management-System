namespace FabricesStoreManagementSystem.SortingConfig;

public class CatalogSorts
{
    public static Expression<Func<Catalog, object>> CatalogResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "status" => catalog => catalog.Status,
            "productscount" => catalog => catalog.ProductsCount,
            "code" => catalog => catalog.CatalogCode,
            "createdat" => catalog => catalog.CreatedAt,
            "id" => catalog => catalog.Id,
            _ => sale => sale.CreatedAt
        };

    public static SortColumnsResponse CatalogSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("الحالة", "status"), new LabelValue("عدد المنتجات", "productscount"),
                new LabelValue("الكود", "code"), new LabelValue("تاريخ الانشاء", "createdat"),
                new LabelValue("المعرف", "id")],
                new LabelValue("تاريخ الانشاء", "createdat")
            );
}