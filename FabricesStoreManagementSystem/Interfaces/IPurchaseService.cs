namespace FabricesStoreManagementSystem.Interfaces;

public interface IPurchaseService
{
    Task<Result<PurchaseResponse>> CreatePurchase(PurchaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchases(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchase(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchaseByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result> RemovePurchase(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdatePurchasePaidAmount(Guid id, PurchaseUpdatePaidRequest request, CancellationToken cancellationToken = default);

}
