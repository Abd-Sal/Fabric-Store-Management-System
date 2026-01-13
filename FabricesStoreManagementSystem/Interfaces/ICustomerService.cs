namespace FabricesStoreManagementSystem.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerResponse>> CreateCustomer(CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> UpdateCustomer(CustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveCustomer(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<CustomerResponse>>> GetCustomers(CancellationToken cancellationToken = default);
    Task<Result<CustomerResponse>> GetCustomer(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSaleByCustomer(Guid id, CancellationToken cancellationToken = default);

}
