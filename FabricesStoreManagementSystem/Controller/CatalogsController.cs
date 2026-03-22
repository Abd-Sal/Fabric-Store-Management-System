namespace FabricesStoreManagementSystem.Controller;

[Route("api/catalogs")]
[ApiController]
[Authorize]
public class CatalogsController(IUnitOfWork unitOfWork, ILogger<CatalogService> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CatalogService> _logger = logger;
    
    [HttpGet("")]
    public async Task<IActionResult> GetCatalogs
    ([FromQuery] PaginationRequest paginationRequest,
    [FromQuery] SortRequest sortRequest,
    [FromQuery] DateRangeRequest dateRangeRequest,
    [FromQuery] SearchRequest searchRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetCatalogs(paginationRequest, sortRequest, dateRangeRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCatalog
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetCatalog(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCatalog
    ([FromBody] CatalogRequest catalogRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.CreateCatalog(catalogRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetCatalog), new {id = result.Value.Id}, result.Value);
    }

    [HttpPost("purchase-catalog")]
    public async Task<IActionResult> PurchaseCatalog
    ([FromBody] CatalogFormPurchaseCatalogRequest catalogFormPurchaseCatalogRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.PurchaseCatalog(catalogFormPurchaseCatalogRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCatalog), new { id = result.Value.Id }, result.Value);
    }

    [HttpPost("by-supplier")]
    public async Task<IActionResult> CreateCatalogBySupplier
    ([FromBody] CatalogFromSupplierRequest catalogFromSupplierRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.CreateCatalog(catalogFromSupplierRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetCatalog), new {id = result.Value.Id}, result.Value);
    }
    
    [HttpPost("assign-catalog")]
    public async Task<IActionResult> AssignCatalog
    ([FromBody] AssignCatalogRequest assignCatalogRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.AssignCatalog(assignCatalogRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/return-catalog")]
    public async Task<IActionResult> ReturnCatalog
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.ReturnCatalog(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveCatalog
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.RemoveCatalog(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/destroy")]
    public async Task<IActionResult> DestroyCatalog
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.DestructionCatalog(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPut("{id:guid}/pay")]
    public async Task<IActionResult> PayForPurchasedCatalog
    ([FromRoute] Guid id,
    PurchaseUpdatePaidRequest purchaseUpdatePaidRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.PayForCatalog(id, purchaseUpdatePaidRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("assigned-catalogs")]
    public async Task<IActionResult> GetAssignedCatalogs
        ([FromQuery] PaginationRequest paginationRequest,
        [FromQuery] SortRequest sortRequest,
        [FromQuery] DateRangeRequest dateRangeRequest,
        [FromQuery] SearchRequest searchRequest,
        [FromQuery] bool includeReturned = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetAssingedCatalogs
            (paginationRequest, sortRequest, dateRangeRequest, searchRequest, includeReturned, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        return Ok(result.Value);
    }

    [HttpGet("{month:int}/customers-has-catalogs-and-not-buy")]
    public async Task<IActionResult> GetCustomersWhoHasCatalogsAndNotBuyByMonthNumber
        ([FromRoute] int month,
        [FromQuery] PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetCustomersWhoHasCatalogsAndNotBuyByMonthNumber(month, paginationRequest, cancellationToken);
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
            SearchDetails = CatalogSearchs.CatalogSearchColumns(),
            SortDetails = CatalogSorts.CatalogSortColumns(),
        };

        return Ok(result);
    }

    [HttpOptions("assigned-catalogs")]
    public async Task<IActionResult> AssignedCatalogsDetails()
    {
        var result = new
        {
            SearchDetails = AssignCatalogSearchs.AssignCatalogSearchColumns(),
            SortDetails = AssignCatalogSorts.AssignCatalogSortColumns(),
        };

        return Ok(result);
    }
}
