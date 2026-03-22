namespace FabricesStoreManagementSystem.Controller;

[Route("api/customers")]
[ApiController]
[Authorize]
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
        return NoContent();
    }

    [HttpGet("{id:guid}/sales")]
    public async Task<IActionResult> SalesByCustomer
            ([FromRoute] Guid id,
            [FromQuery] PaginationRequest paginationRequest,
            [FromQuery] SearchInvoiceNumberRequest searchInvoiceNumberRequest,
            [FromQuery] DateRangeRequest dateRangeRequest,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetSalesByCustomer
            (id, paginationRequest, searchInvoiceNumberRequest, dateRangeRequest, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/catalogs")]
    public async Task<IActionResult> CatalogsByCustomer
            ([FromRoute] Guid id,
            [FromQuery] PaginationRequest paginationRequest,
            [FromQuery] SearchCatalogByCodeRequest searchCatalogByCodeRequest,
            [FromQuery] DateRangeRequest dateRangeRequest,
            [FromQuery] bool includeReturned = false,
            CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetCustomerCatalogs
            (id, paginationRequest, searchCatalogByCodeRequest, dateRangeRequest, includeReturned, cancellationToken: cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetCustomersForBill
        ([FromQuery] CustomerSearchForBillRequest search,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.CustomerService.GetCustomerForBill(search, cancellationToken);
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
            SearchDetails = CustomerSearchs.CustomerSearchColumns(),
            SortDetails = CustomerSorts.CustomerSortColumns(),
        };
        return Ok(result);
    }

}