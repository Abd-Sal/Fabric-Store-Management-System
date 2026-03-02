namespace FabricesStoreManagementSystem.Interfaces;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProduct(ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetProduct(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<ProductResponse>>> GetProducts(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<ProductWithInventoryResponse>> GetProductInventory(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<StockTransactionResponse>>> GetProductStockTransactions(Guid id, PaginationRequest paginationRequest, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SaleResponse>>> GetSalesByProduct(Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PurchaseResponse>>> GetPurchasesByProduct(Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<List<ProductWithInventoryResponse>>> GetProductsForBill(SearchProductBillByCodeRequest searchCode, CancellationToken cancellationToken = default);
    Task<Result<List<ProductResponse>>> GetProductsByCode(ProductCodeRequest request, CancellationToken cancellationToken = default);
}
