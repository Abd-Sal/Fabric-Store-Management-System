namespace FabricesStoreManagementSystem.Controller;

[Route("api/products")]
[ApiController]
public class ProductsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct
        ([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProduct(id, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetProducts
        ([FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest? dateRangeRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProducts(paginationRequest, sortRequest, dateRangeRequest, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateProduct
        ([FromBody]ProductRequest productRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.CreateProduct(productRequest, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetProducts), new {id = result.Value.Id}, result.Value);
    }

    [HttpGet("{id:guid}/inventory")]
    public async Task<IActionResult> ProductInventory
        ([FromRoute]Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProductInventory(id, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/stock-transactions")]
    public async Task<IActionResult> ProductStockTransactions
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetProductStockTransactions(id, paginationRequest, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/sales")]
    public async Task<IActionResult> ProductSales
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest? dateRangeRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetSalesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/purchases")]
    public async Task<IActionResult> ProductPurchases
        ([FromRoute]Guid id,
        [FromQuery]PaginationRequest paginationRequest,
        [FromQuery]SortRequest sortRequest,
        [FromQuery]DateRangeRequest? dateRangeRequest,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.ProductService.GetPurchasesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, cancellationToken);
        if(result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }
}
