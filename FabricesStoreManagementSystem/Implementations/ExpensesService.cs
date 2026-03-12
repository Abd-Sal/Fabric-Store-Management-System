namespace FabricesStoreManagementSystem.Implementations;

public class ExpenseService(AppDbContext appDbContext, ILogger<ExpenseService> logger) : IExpenseService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<ExpenseService> _logger = logger;

    public async Task<Result<ExpenseResponse>> CreateExpense
        (ExpenseRequest expenseRequest, CancellationToken cancellationToken = default)
    {
        var expense = new Expense
        {
            Message = expenseRequest.Message,
            SyrianAmount = expenseRequest.SyrianAmount,
            DollarPriceInSyr = expenseRequest.DollarPriceInSyr,
        };

        var payment = new Payment
        {
            Amount = expense.DollarAmount,
            PayMethod = PaymentMethod.Cash,
            ReferenceID = expense.Id,
            ReferenceType = ReferenceTypes.Expense
        };

        await _appDbContext.Expenses.AddAsync(expense, cancellationToken);
        _logger.LogInformation("create expense({id})", expense.Id);
        await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        _logger.LogInformation("create payment({id})", payment.Id);

        return Result.Success(expense.ToExpenseResponse());
    }

    public async Task<Result<ExpenseResponse>> GetExpense
        (Guid id, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Expenses.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is not { } expense)
            return Result.Failure<ExpenseResponse>(ExpenseErrors.NotFound);
        return Result.Success(expense.ToExpenseResponse());
    }

    public async Task<Result<PaginatedList<ExpenseResponse>>> GetExpenses
        (PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, SortRequest sortRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Expenses.AsNoTracking();

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.CreatedAt >= utcFrom && x.CreatedAt <= utcTo);
        }

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.ExpenseResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(ExpenseSorts.ExpenseResponseSort(sortRequest));
        else
            query = query.OrderByDescending(ExpenseSorts.ExpenseResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToExpenseResponse());

        var response = await PaginatedList<ExpenseResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> RemoveExpense
        (Guid id, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Expenses.FindAsync(id, cancellationToken) is not { } expense)
            return Result.Failure(ExpenseErrors.NotFound);

        var checkDate = expense.CreatedAt.AddDays(3) >= DateTime.UtcNow;
        if (!checkDate)
        {
            _logger.LogInformation("Cannot Remove expense({id}) cause it created sience 3 days or more", id);
            return Result.Failure(ExpenseErrors.ExpenseIsVeryOld);
        }

        var payment = await _appDbContext.Payments.SingleAsync(x => x.ReferenceID == id, cancellationToken);
        _appDbContext.Payments.Remove(payment);
        _logger.LogInformation("Remove payment({id})", payment.Id);
        _appDbContext.Expenses.Remove(expense);
        _logger.LogInformation("Remove expense({id})", id);
        return Result.Success();
    }
}