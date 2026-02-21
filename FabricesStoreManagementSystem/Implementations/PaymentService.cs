namespace FabricesStoreManagementSystem.Implementations;

public class PaymentService(AppDbContext appDbContext, ILogger<PaymentService> logger) : IPaymentService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<PaymentService> _logger = logger;

    public async Task<Result<PaginatedList<PaymentResponse>>> GetPayments
        (PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, Guid searchReferanceID, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Payments.AsNoTracking();

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.PaidAt >= utcFrom && x.PaidAt <= utcTo);
        }

        if (searchReferanceID != Guid.Empty)
            query = query
                .Where(x => x.ReferenceID.ToString().ToLower().Contains(searchReferanceID.ToString()!));

        query = query.OrderByDescending(x => x.PaidAt);

        var result = query.Select(x => x.ToPaymentResponse());

        var response = await PaginatedList<PaymentResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }
}
