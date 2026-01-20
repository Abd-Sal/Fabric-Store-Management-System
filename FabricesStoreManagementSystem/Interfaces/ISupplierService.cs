namespace FabricesStoreManagementSystem.Interfaces;

public interface ISupplierService
{
    Task<Result<SupplierResponse>> CreateSupplier(SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateSupplier(Guid id, SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleSupplierStatus(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SupplierResponse>>> GetSuppliers(PaginationRequest paginationRequest, SortRequest sortRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> GetSupplier(Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchaseBySupplier(Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, CancellationToken cancellationToken = default);
}
