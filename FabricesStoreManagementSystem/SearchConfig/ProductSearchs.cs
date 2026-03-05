namespace FabricesStoreManagementSystem.SearchConfig;

public static class ProductSearchs
{
    public static IQueryable<Product> ProductResponseSearch(this IQueryable<Product> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "id" => query.Where(x => EF.Functions.Like(x.Id.ToString(), $"%{searchRequest.Search}%")),
            "code" => query.Where(x => EF.Functions.Like(x.Code, $"%{searchRequest.Search}%")),
            "color" => query.Where(x => EF.Functions.Like(x.Color, $"%{searchRequest.Search}%")),
            "unit" => query.Where(x => EF.Functions.Like(x.Unit, $"%{searchRequest.Search}%")),
            "name" => query.Where(x => EF.Functions.Like(x.Name ?? "", $"%{searchRequest.Search}%")),
            "material" => query.Where(x => EF.Functions.Like(x.Material ?? "", $"%{searchRequest.Search}%")),
            "productcode" => query.Where(x => EF.Functions.Like(x.Code + "-" + x.Color, $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.Code + "-" + x.Color, $"%{searchRequest.Search}%"))
        };

    public static SearchColumnsResponse ProductSortColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("الوحدة", "unit"), new LabelValue("الكود", "code"),
                new LabelValue("اللون", "color"), new LabelValue("مادة الصنع", "material"),
                new LabelValue("المعرف", "id"), new LabelValue("الاسم", "name"),
                new LabelValue("الكود كامل", "productcode")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("الكود كامل", "productCode")
        );
}