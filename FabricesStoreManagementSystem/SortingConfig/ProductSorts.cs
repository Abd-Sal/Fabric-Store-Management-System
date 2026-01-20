namespace FabricesStoreManagementSystem.SortingConfig;

public class ProductSorts
{
    public static Expression<Func<Product, object>> ProductResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "name" => product => product.Name ?? "",
            "code" => product => product.Code,
            "color" => product => product.Color,
            "productcode" => product => product.ProductCode,
            "createdat" => product => product.CreatedAt,
            "id" => product => product.Id,
            _ => product => product.CreatedAt
        };

}
