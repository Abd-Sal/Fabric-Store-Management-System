namespace FabricesStoreManagementSystem.SearchConfig;

public class ProductSearchs
{
    public static Expression<Func<Product, object>> ProductResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "unit" => product => product.Unit,
            "code" => product => product.ProductCode,
            "color" => product => product.Color,
            "material" => product => product.Material ?? "",
            "id" => product => product.Id,
            "name" => product => product.Name ?? "",
            _ => product => product.ProductCode
        };

    public static SearchColumnsResponse ProductSortColumns()
    => new SearchColumnsResponse(
            ["unit", "code", "color", "material", "id", "name"],
            "code"
        );
}
