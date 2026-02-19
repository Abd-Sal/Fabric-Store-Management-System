namespace FabricesStoreManagementSystem.Controller;

[Route("api/products")]
[ApiController]
public class ProductsController(IUnitOfWork unitOfWork, ILogger<ProductService> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<ProductService> _logger = logger;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct
        ([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProduct(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetProducts
        ([FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest dateRangeRequest,
        [FromQuery]SearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProducts(paginationRequest, sortRequest, dateRangeRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateProduct
        ([FromBody]ProductRequest productRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.CreateProduct(productRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetProducts), new {id = result.Value.Id}, result.Value);
    }

    [HttpGet("{id:guid}/inventory")]
    public async Task<IActionResult> ProductInventory
        ([FromRoute]Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProductInventory(id, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/stock-transactions")]
    public async Task<IActionResult> ProductStockTransactions
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProductStockTransactions(id, paginationRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/sales")]
    public async Task<IActionResult> ProductSales
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest dateRangeRequest,
        [FromQuery]SearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetSalesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/purchases")]
    public async Task<IActionResult> ProductPurchases
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest dateRangeRequest,
        [FromQuery]SearchRequest searchRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetPurchasesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("{error}: {desc}", result.Error.Code, result.Error.Description);
            return result.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchForProductsForBill
        ([FromQuery] SearchProductBillByCodeRequest searchCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProductsForBill(searchCode, cancellationToken);
        if (result.IsFailure)
        {
            _logger.LogError("Error happen: {err}", result.Error);
            return result.ToProblem();
        }
        return Ok(result.Value);
    }

    [HttpOptions("")]
    public async Task<IActionResult> Details()
    {
        var result = new
        {
            SearchDetails = ProductSearchs.ProductSortColumns(),
            SortDetails = ProductSorts.ProductSortColumns(),
        };

        return Ok(result);
    }
}
