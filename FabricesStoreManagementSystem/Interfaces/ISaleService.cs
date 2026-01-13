namespace FabricesStoreManagementSystem.Interfaces;

public interface ISaleService
{
    Task<Result<SaleResponse>> CreateSale(SaleRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSales(CancellationToken cancellationToken = default);
    Task<Result<SaleResponse>> GetSale(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SaleResponse>> GetSaleByInvoiceNumber(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<List<SaleResponse>>> GetSaleByRangeDate(DateRangeRequest dateRange, CancellationToken cancellationToken = default);
}