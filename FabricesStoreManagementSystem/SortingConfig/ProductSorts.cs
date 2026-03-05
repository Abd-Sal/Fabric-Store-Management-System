namespace FabricesStoreManagementSystem.SortingConfig;

public class ProductSorts
{
    public static Expression<Func<Product, object>> ProductResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name ?? "",
            "code" => product => product.Code,
            "color" => product => product.Color,
            "productcode" => product => product.Code + " " + product.Color,
            "createdat" => product => product.CreatedAt,
            "id" => product => product.Id,
            _ => product => product.CreatedAt
        };

    public static SortColumnsResponse ProductSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("الاسم", "name"), new LabelValue("اللون", "color"),
                new LabelValue("الكود", "code"), new LabelValue("كود و اللون", "productcode"),
                new LabelValue("تاريخ الانشاء", "createdat"), new LabelValue("المعرف", "id")],
                new LabelValue("تاريخ الانشاء", "createdat")
            );
}
