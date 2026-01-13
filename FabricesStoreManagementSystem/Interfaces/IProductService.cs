namespace FabricesStoreManagementSystem.Interfaces;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProduct(ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProduct(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<ProductResponse>>> GetProducts(CancellationToken cancellationToken = default);
    Task<Result<ProductWithInventoryResponse>> GetProductInventory(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<ProductStockTransactionsResponse>>> GetProductStockTransactions(Guid id, CancellationToken cancellationToken = default);
}
