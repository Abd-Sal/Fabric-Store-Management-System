namespace FabricesStoreManagementSystem.Controller;

[Route("api/sales")]
[ApiController]
public class SalesController(IUnitOfWork unitOfWork, ILogger<SaleService> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<SaleService> _logger = logger;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSale
    ([FromRoute]Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.GetSale(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetSales
    ([FromQuery] PaginationRequest paginationRequest,
    [FromQuery] SortRequest sortRequest,
    [FromQuery] DateRangeRequest dateRangeRequest,
    [FromQuery] SearchRequest searchRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.GetSales(paginationRequest, sortRequest, dateRangeRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateSale
    ([FromBody] SaleRequest saleRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.CreateSale(saleRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetSale), new {id = result.Value.Id}, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveSale
    ([FromRoute]Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.RemoveSale(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{id:guid}/pay")]
    public async Task<IActionResult> PayForSale
    ([FromRoute]Guid id,
    [FromBody]SaleUpdatePaidRequest saleUpdatePaidRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.UpdateSalePaidAmount(id, saleUpdatePaidRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("invoice")]
    public async Task<IActionResult> SaleByInvoiceNumber
    ([FromQuery(Name = "invoice-number")]string invoice,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SaleService.GetSaleByInvoiceNumber(invoice, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpOptions("")]
    public async Task<IActionResult> Details()
    {
        var result = new
        {
            SearchDetails = SaleSearchs.SaleSearchColumns(),
            SortDetails = SaleSorts.SaleSortColumns(),
        };

        return Ok(result);
    }
}
