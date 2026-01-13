namespace FabricesStoreManagementSystem.Interfaces;

public interface ISupplierService
{
    Task<Result<SupplierResponse>> CreateSupplier(SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> UpdateSupplier(SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveSupplier(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<SupplierResponse>>> GetSuppliers(CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> GetSupplier(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseResponse>>> GetPurchaseBySupplier(Guid id, CancellationToken cancellationToken = default);
}
