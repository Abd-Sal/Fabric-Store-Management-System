namespace FabricesStoreManagementSystem.Controller;

[Route("api/customers")]
[ApiController]
public class CustomersController(IUnitOfWork unitOfWork, ILogger<CustomerService> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CustomerService> _logger = logger;

    [HttpGet("{id:guid}/{state:bool?}")]
    public async Task<IActionResult> GetCustomer
        ([FromRoute] Guid id,
        [FromRoute] bool? state,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetCustomer
            (id, includeOnlyActive: state.HasValue ? (bool)state : true, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return Ok(result.Value);
    }

    [HttpGet("{state:bool?}")]
    public async Task<IActionResult> GetCustomers
        ([FromRoute] bool? state,
        [FromQuery] PaginationRequest paginatinoRequest,
        [FromQuery] SortRequest sortRequest,
        [FromQuery] SearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetCustomers
            (paginatinoRequest, sortRequest, searchRequest, includeOnlyActive: state.HasValue ? (bool)state : true, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateCustomer
        ([FromBody] CustomerRequest CustomerRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.CreateCustomer
            (CustomerRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value.Id }, result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateCustomer
            ([FromRoute] Guid id,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.ToggleCustomerStatus
            (id, false, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return NoContent();
    }

    [HttpPut("{id:guid}/activating")]
    public async Task<IActionResult> ActivateCustomer
            ([FromRoute] Guid id,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.ToggleCustomerStatus
            (id, true, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer
            ([FromRoute] Guid id,
            [FromBody] CustomerRequest CustomerRequest,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.UpdateCustomer
           (id, CustomerRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return NoContent();
    }

    [HttpGet("{id:guid}/sales")]
    public async Task<IActionResult> PurchasesByCustomer
            ([FromRoute] Guid id,
            [FromQuery] PaginationRequest paginatinoRequest,
            [FromQuery] SortRequest sortRequest,
            [FromQuery] SearchRequest searchRequest,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetSalesByCustomer
            (id, paginatinoRequest, sortRequest, searchRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return Ok(result.Value);
    }

    [HttpOptions("")]
    public async Task<IActionResult> Details()
    {
        var result = new
        {
            SearchDetails = CustomerSearchs.CustomerSortColumns(),
            SortDetails = CustomerSorts.CustomerSortColumns(),
        };
        _logger.LogInformation("request: {req}", HttpContext.Request);
        return Ok(result);
    }

}