namespace FabricesStoreManagementSystem.Controller;

[Route("api/expenses")]
[ApiController]
[Authorize]
public class ExpensesController(IUnitOfWork unitOfWork, ILogger<ExpensesController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ExpensesController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllExpenses
        ([FromQuery] PaginationRequest paginationRequest,
        [FromQuery] DateRangeRequest dateRangeRequest,
        [FromQuery] SearchRequest searchRequest,
        [FromQuery] SortRequest sortRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ExpenseService.GetExpenses(paginationRequest, dateRangeRequest, sortRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpense
        ([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ExpenseService.GetExpense(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateExpense
        ([FromBody] ExpenseRequest expenseRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ExpenseService.CreateExpense(expenseRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetExpense), new { id = result.Value.Id }, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveExpense
        ([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ExpenseService.RemoveExpense(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpOptions("")]
    public async Task<IActionResult> EndpointDetails
        (CancellationToken cancellationToken = default)
    {
        var result = new
        {
            SearchDetails = ExpenseSearchs.ExpenseSearchColumns(),
            SortDetails = ExpenseSorts.ExpenseSortColumns(),
        };

        return Ok(result);
    }
}
