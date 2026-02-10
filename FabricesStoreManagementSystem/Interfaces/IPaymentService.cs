namespace FabricesStoreManagementSystem.Interfaces;

public interface IPaymentService
{
    Task<Result<PaginatedList<PaymentResponse>>> GetPayments(PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, Guid searchReferanceID, CancellationToken cancellationToken = default);
}
