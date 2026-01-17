namespace FabricesStoreManagementSystem.Interfaces;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProduct(ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProduct(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<ProductResponse>>> GetProducts(CancellationToken cancellationToken = default);
    Task<Result<ProductWithInventoryResponse>> GetProductInventory(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ProductStockTransactionsResponse>> GetProductStockTransactions(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSalesByProduct(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseResponse>>> GetPurchasesByProduct(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseResponse>>> GetPurchasesByProductAndDateRange(Guid id, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSalesByProductAndDateRange(Guid id, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default);
}
