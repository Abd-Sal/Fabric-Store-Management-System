namespace FabricesStoreManagementSystem.SearchConfig;

public static class CustomerSearchs
{
    public static IQueryable<Customer> CustomerResponseSearch(this IQueryable<Customer> query, SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "firstname" => query.Where(x => EF.Functions.Like(x.FirstName, $"%{searchRequest.Search}%")),
            "lastname" => query.Where(x => EF.Functions.Like(x.LastName, $"%{searchRequest.Search}%")),
            "address" => query.Where(x => x.Address != null && EF.Functions.Like(x.Address, $"%{searchRequest.Search}%")),
            "email" => query.Where(x => x.Email != null && EF.Functions.Like(x.Email, $"%{searchRequest.Search}%")),
            "phone" => query.Where(x => x.Phone != null && EF.Functions.Like(x.Phone, $"%{searchRequest.Search}%")),
            "id" => query.Where(x => EF.Functions.Like(x.Id.ToString(), $"%{searchRequest.Search}%")),
            "name" => query.Where(x => EF.Functions.Like(string.Concat(x.FirstName, " ", x.LastName), $"%{searchRequest.Search}%")),
            _ => query.Where(x => EF.Functions.Like(string.Concat(x.FirstName, " ", x.LastName), $"%{searchRequest.Search}%"))
        };

    public static SearchColumnsResponse CustomerSortColumns()
    => new SearchColumnsResponse(
            new List<LabelValue>{
                new LabelValue("العنوان", "address"),
                new LabelValue("البريد الالكتروني", "email"), new LabelValue("رقم الهاتف", "phone"),
                new LabelValue("المعرف", "id"), new LabelValue("الاسم الأول", "firstname"),
                new LabelValue("الاسم الأخير", "lastname"), new LabelValue("الاسم الكامل", "name")
            }.OrderBy(x => x.Label).ToArray(),
            new LabelValue("الاسم الكامل", "name")
        );

}