namespace FabricesStoreManagementSystem.Controller;

[Route("api/purchases")]
[ApiController]
public class PurchasesController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchase
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.GetPurchase(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetPurchases
    ([FromQuery] PaginationRequest paginationRequest,
    [FromQuery] SortRequest sortRequest,
    [FromQuery] DateRangeRequest? dateRangeRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.GetPurchases(paginationRequest, sortRequest, dateRangeRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePurchase
    ([FromBody] PurchaseRequest purchaseRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.CreatePurchase(purchaseRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetPurchase), new { id = result.Value.Id }, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemovePurchase
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.RemovePurchase(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/pay")]
    public async Task<IActionResult> PayForPurchase
    ([FromRoute] Guid id,
    PurchaseUpdatePaidRequest purchaseUpdatePaidRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.UpdatePurchasePaidAmount(id, purchaseUpdatePaidRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("invoice")]
    public async Task<IActionResult> PurchaseByInvoiceNumber
    ([FromQuery(Name = "invoice-number")] string invoice,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.PurchaseService.GetPurchaseByInvoiceNumber(invoice, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

}
