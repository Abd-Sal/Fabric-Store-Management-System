namespace FabricesStoreManagementSystem.Controller;

[Route("api/catalogs")]
[ApiController]
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
        _logger.LogError("get catalogs");
        
        // Validate SearchRequest if provided
        if (searchRequest is not null)
        {
            var validator = new SearchValidations();
            var validationResult = await validator.ValidateAsync(searchRequest, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(new { errors });
            }
        }
        
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
        _logger.LogError("get catalog({id})", id);
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
        _logger.LogError("create catalog by stock");
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
        _logger.LogError("purchase catalog by stock");
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
        _logger.LogError("create catalog by supplier");
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
        _logger.LogError("assign catalog({id})", assignCatalogRequest.CatalogID);
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
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        _logger.LogError("return catalog({id})", id);
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
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        _logger.LogError("remove catalog({id})", id);
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
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        _logger.LogError("destroy catalog({id})", id);
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
        _logger.LogInformation("pay for purchased catalog({id})", id);
        var result = await _unitOfWork.CatalogService.PayForCatalog(id, purchaseUpdatePaidRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpOptions("")]
    public async Task<IActionResult> Details()
    {
        var result = new
        {
            SearchDetails = CatalogSearchs.CatalogSortColumns(),
            SortDetails = CatalogSorts.CatalogSortColumns(),
        };
        return Ok(result);
    }
}
