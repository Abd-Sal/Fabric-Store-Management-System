namespace FabricesStoreManagementSystem.SearchConfig;

public class CustomerSearchs
{
    public static Expression<Func<Customer, object>> CustomerResponseSearch(SearchRequest searchRequest)
        => searchRequest.SearchColumn?.ToLower() switch
        {
            "name" => customer => $"{customer.FirstName} {customer.LastName}",
            "address" => customer => customer.Address ?? "",
            "email" => customer => customer.Email ?? "",
            "phone" => customer => customer.Phone ?? "",
            "id" => customer => customer.Id,
            _ => customer => new { customer.FirstName, customer.LastName }
        };

    public static SearchColumnsResponse CustomerSortColumns()
    => new SearchColumnsResponse(
            ["name", "address", "email", "phone", "id"],
            "name"
        );

}