namespace FabricesStoreManagementSystem.Controller;

[Route("api/payments")]
[ApiController]
public class PaymentsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet]
    public async Task<IActionResult> GetAllPayments
        ([FromQuery] PaginationRequest paginationRequest,
        [FromQuery] DateRangeRequest dateRangeRequest,
        [FromQuery] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PaymentService.GetPayments(paginationRequest, dateRangeRequest, id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }
}
