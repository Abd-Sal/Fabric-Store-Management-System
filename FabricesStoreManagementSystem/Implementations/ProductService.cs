namespace FabricesStoreManagementSystem.Implementations;

public class ProductService(AppDbContext appDbContext) : IProductService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<ProductResponse>> CreateProduct
        (ProductRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Code == request.Code && x.Color == request.Color, cancellationToken)))
            return Result.Failure<ProductResponse>(ProductErrors.CodeWithColorConflict);
        var product = new Product
        {
            Name = request.Name,
            Code = request.Code,
            Color = request.Color,
            Unit = request.Unit,
            Material = request.Material
        };

        await _appDbContext.Products.AddAsync(product, cancellationToken);
        return Result.Success(product.ToProductResponse());
    }

    public async Task<Result<ProductResponse>> GetProduct
        (Guid id, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Products.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken) is not { } product)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);
        return Result.Success(product.ToProductResponse());
    }

    public async Task<Result<ProductWithInventoryResponse>> GetProductInventory
        (Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.Inventory)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<ProductWithInventoryResponse>(ProductErrors.NotFound);

        return Result.Success(product.ToProductWithInventoryResponse());
    }

    public async Task<Result<List<ProductResponse>>> GetProducts
        (CancellationToken cancellationToken = default)
    {
        var result = await _appDbContext.Products
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToProductResponse())
            .ToListAsync(cancellationToken);
        return Result.Success(result);
    }

    public async Task<Result<ProductStockTransactionsResponse>> GetProductStockTransactions
        (Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.Inventory)
            .Include(x => x.StockTransactions)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (product is null)
            return Result.Failure<ProductStockTransactionsResponse>(ProductErrors.NotFound);
        return Result.Success(product.ToProductStockTransactionsResponse());
    }

    public async Task<Result<List<SaleResponse>>> GetSalesByProduct
        (Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.Sale)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<List<SaleResponse>>(ProductErrors.NotFound);

        if (!product.SaleItems.Any())
            return Result.Success(new List<SaleResponse>());

        var sales = product.SaleItems
            .Select(x => x.Sale.ToSaleResponseWithNoItems())
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Result.Success(sales);
    }

    public async Task<Result<List<PurchaseResponse>>> GetPurchasesByProduct
        (Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Purchase)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<List<PurchaseResponse>>(ProductErrors.NotFound);

        if (!product.PurchaseItems.Any())
            return Result.Success(new List<PurchaseResponse>());

        var purchases = product.PurchaseItems
            .Select(x => x.Purchase.ToPurchaseResponseWithoutItems())
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Result.Success(purchases);
    }

    public async Task<Result<List<PurchaseResponse>>> GetPurchasesByProductAndDateRange
        (Guid id, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Purchase)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<List<PurchaseResponse>>(ProductErrors.NotFound);

        if (!product.PurchaseItems.Any())
            return Result.Success(new List<PurchaseResponse>());

        var purchases = product.PurchaseItems
            .Select(x => x.Purchase.ToPurchaseResponseWithoutItems())
            .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From && 
                    DateOnly.Parse(x.CreatedAt.ToString())<= dateRangeRequest.To)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Result.Success(purchases);
    }

    public async Task<Result<List<SaleResponse>>> GetSalesByProductAndDateRange
        (Guid id, DateRangeRequest dateRangeRequest, CancellationToken cancellationToken = default)
    {
        var product = await _appDbContext.Products.AsNoTracking()
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.Sale)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<List<SaleResponse>>(ProductErrors.NotFound);

        if (!product.SaleItems.Any())
            return Result.Success(new List<SaleResponse>());

        var sales = product.SaleItems
            .Select(x => x.Sale.ToSaleResponseWithNoItems())
            .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                    DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return Result.Success(sales);
    }
}
