namespace FabricesStoreManagementSystem.Interfaces;

public interface IPurchaseService
{
    Task<Result<PurchaseItemsResponse>> CreatePurchase(PurchaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseResponse>>> GetPurchases(CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchase(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PurchaseResponse>> GetPurchaseByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseResponse>>> GetPurchaseByRangeDate(DateRangeRequest dateRange, CancellationToken cancellationToken = default);
}
