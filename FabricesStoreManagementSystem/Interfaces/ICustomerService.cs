namespace FabricesStoreManagementSystem.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateCustomer(CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateCustomer(Guid id, CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleCustomerStatus(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<CustomerResponse>>> GetCustomers(bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> GetCustomer(Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSaleByCustomer(Guid id, CancellationToken cancellationToken = default);
}
