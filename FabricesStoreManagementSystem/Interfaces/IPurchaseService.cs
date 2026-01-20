namespace FabricesStoreManagementSystem.Interfaces;

public interface IPurchaseService
{
    Task<Result<PurchaseResponse>> CreatePurchase(PurchaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchases(PaginationRequest paginationRequest, SortRequest sortRequest, CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchase(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchaseByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchaseByRangeDate(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRange, CancellationToken cancellationToken = default);
}
