namespace FabricesStoreManagementSystem.Interfaces;

public interface IExpenseService
{
    Task<Result<PaginatedList<ExpenseResponse>>> GetExpenses(PaginationRequest paginationRequest, DateRangeRequest dateRangeRequest, SortRequest sortRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default);
    Task<Result<ExpenseResponse>> GetExpense(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ExpenseResponse>> CreateExpense(ExpenseRequest expenseRequest, CancellationToken cancellationToken = default);
    Task<Result> RemoveExpense(Guid id, CancellationToken cancellationToken = default);
}
