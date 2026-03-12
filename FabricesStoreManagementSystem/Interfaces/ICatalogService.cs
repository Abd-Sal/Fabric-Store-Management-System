namespace FabricesStoreManagementSystem.Interfaces;

public interface ICatalogService
{
    Task<Result> PayForCatalog(Guid id, PurchaseUpdatePaidRequest request, CancellationToken cancellationToken = default);
    Task<Result<CatalogResponse>> PurchaseCatalog(CatalogFormPurchaseCatalogRequest request, CancellationToken cancellationToken = default);
    Task<Result<CatalogResponse>> CreateCatalog(CatalogRequest request, CancellationToken cancellationToken = default);
    Task<Result<CatalogResponse>> CreateCatalog(CatalogFromSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveCatalog(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AssignCatalogResponse>> AssignCatalog(AssignCatalogRequest request, CancellationToken cancellationToken = default);
    Task<Result<AssignCatalogResponse>> ReturnCatalog(Guid assignID, CancellationToken cancellationToken = default);
    Task<Result> DestructionCatalog(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<CatalogResponse>>> GetCatalogs(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<CatalogResponse>> GetCatalog(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<AssignCatalogResponse>>> GetAssingedCatalogs(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, bool includeReturned = false, CancellationToken cancellationToken = default);
}
