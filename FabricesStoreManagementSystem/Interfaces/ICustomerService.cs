namespace FabricesStoreManagementSystem.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateCustomer(CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateCustomer(Guid id, CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleCustomerStatus(Guid id, bool? state, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<CustomerResponse>>> GetCustomers(PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest? searchRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> GetCustomer(Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SaleResponse>>> GetSalesByCustomer(Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest? searchRequest, CancellationToken cancellationToken = default);
}
