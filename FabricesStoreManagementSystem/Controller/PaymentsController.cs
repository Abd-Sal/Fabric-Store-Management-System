namespace FabricesStoreManagementSystem.Controller;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IUnitOfWork unitOfWork, ILogger<PaymentsController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<PaymentsController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> GetAllPayments
        ([FromQuery] PaginationRequest paginationRequest,
        [FromQuery] DateRangeRequest dateRangeRequest,
        [FromQuery] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PaymentService.GetPayments(paginationRequest, dateRangeRequest, id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return Ok(result.Value);
    }
}
