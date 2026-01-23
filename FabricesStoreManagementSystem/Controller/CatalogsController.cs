namespace FabricesStoreManagementSystem.Controller;

[Route("api/catalogs")]
[ApiController]
public class CatalogsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("")]
    public async Task<IActionResult> GetCatalogs
    ([FromQuery] PaginationRequest paginationRequest,
    [FromQuery] SortRequest sortRequest,
    [FromQuery] DateRangeRequest? dateRangeRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetCatalogs(paginationRequest, sortRequest, dateRangeRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCatalog
    ([FromRoute] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.GetCatalog(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCatalog
    ([FromBody] CatalogRequest catalogRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.CreateCatalog(catalogRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetCatalog), new {id = result.Value.Id}, result.Value);
    }

    [HttpPost("by-supplier")]
    public async Task<IActionResult> CreateCatalogBySupplier
    ([FromBody] CatalogFromSupplierRequest catalogFromSupplierRequest,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.CreateCatalog(catalogFromSupplierRequest, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
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
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/return-catalog")]
    public async Task<IActionResult> ReturnCatalog
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.ReturnCatalog(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveCatalog
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.RemoveCatalog(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/destroy")]
    public async Task<IActionResult> DestroyCatalog
    ([FromBody] Guid id,
    CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CatalogService.DestructionCatalog(id, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
