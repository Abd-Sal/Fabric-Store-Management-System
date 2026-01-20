namespace FabricesStoreManagementSystem.Interfaces;

public interface ISaleService
{
    Task<Result<SaleResponse>> CreateSale(SaleRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateSalePaidAmount(Guid id, SaleUpdatePaidRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveSale(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SaleResponse>>> GetSales(PaginationRequest paginationRequest, SortRequest sortRequest, CancellationToken cancellationToken = default);
    Task<Result<SaleResponse>> GetSale(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SaleResponse>> GetSaleByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<SaleResponse>>> GetSaleByRangeDate(PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRange, CancellationToken cancellationToken = default);
}