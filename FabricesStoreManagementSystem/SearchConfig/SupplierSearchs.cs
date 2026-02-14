namespace FabricesStoreManagementSystem.SearchConfig;

public static class SupplierSearchs
{
    public static IQueryable<Supplier> SupplierResponseSearch(this IQueryable<Supplier> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "name" => query.Where(x => EF.Functions.Like(x.Name, $"%{searchRequest.Search}%")),
            "address" => query.Where(x => x.Address != null && EF.Functions.Like(x.Address, $"%{searchRequest.Search}%")),
            "email" => query.Where(x => x.Email != null && EF.Functions.Like(x.Email, $"%{searchRequest.Search}%")),
            "phone" => query.Where(x => x.Phone != null && EF.Functions.Like(x.Phone, $"%{searchRequest.Search}%")),
            "id" => query.Where(x => EF.Functions.Like(x.Id.ToString(), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(x.Name, $"%{searchRequest.Search}%"))
        };

    public static SearchColumnsResponse SupplierSortColumns()
    => new SearchColumnsResponse(
            [new LabelValue("الاسم", "name"), new LabelValue("العنوان", "address"),
            new LabelValue("البريد الالكتروني", "email"), new LabelValue("رقم الهاتف", "phone"),
            new LabelValue("المعرف", "id")],
            new LabelValue("الاسم", "name")
        );
}
