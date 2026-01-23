namespace FabricesStoreManagementSystem.Controller;

[Route("api/suppliers")]
[ApiController]
public class SuppliersController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("{id:guid}/{state:bool?}")]
    public async Task<IActionResult> GetSupplier
        ([FromRoute]Guid id,
        [FromRoute]bool? state,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.GetSupplier
            (id, includeOnlyActive: state.HasValue ? (bool)state : true, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("{state:bool?}")]
    public async Task<IActionResult> GetSuppliers
        ([FromRoute] bool? state,
        [FromQuery] PaginationRequest paginatinoRequest,
        [FromQuery] SortRequest sortRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.GetSuppliers
            (paginatinoRequest, sortRequest, includeOnlyActive: state.HasValue ? (bool)state : true, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateSupplier
        ([FromBody]SupplierRequest supplierRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.CreateSupplier
            (supplierRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetSupplier), new {id = result.Value.Id}, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateSupplier
            ([FromRoute]Guid id,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.ToggleSupplierStatus
            (id, false, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/activating")]
    public async Task<IActionResult> ActivateSupplier
            ([FromRoute]Guid id,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.ToggleSupplierStatus
            (id, true, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSupplier
            ([FromRoute]Guid id,
            [FromBody] SupplierRequest supplierRequest,
            CancellationToken cancellationToken = default)
    {
         var result = await _unitOfWork.SupplierService.UpdateSupplier
            (id, supplierRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/purchases")]
    public async Task<IActionResult> PurchasesBySupplier
            ([FromRoute]Guid id,
            [FromQuery] PaginationRequest paginatinoRequest,
            [FromQuery] SortRequest sortRequest,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.SupplierService.GetPurchasesBySupplier
            (id, paginatinoRequest, sortRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }
}