namespace FabricesStoreManagementSystem.Interfaces;

public interface ISupplierService
{
    Task<Result<SupplierResponse>> CreateSupplier(SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateSupplier(Guid id, SupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleSupplierStatus(Guid id, bool? state, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SupplierResponse>>> GetSuppliers(PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest searchRequest, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> GetSupplier(Guid id, bool includeOnlyActive = true, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchasesBySupplier(Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, SearchInvoiceNumberRequest invoiceNumberRequest, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default);
    Task<Result<List<SupplierResponse>>> GetSupplierForBill(SupplierSearchForBillRequest request, CancellationToken cancellationToken = default);
}
