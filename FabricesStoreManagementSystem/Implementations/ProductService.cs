namespace FabricesStoreManagementSystem.Implementations;

public class ProductService(AppDbContext appDbContext, ILogger<ProductService> logger) : IProductService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<ProductService> _logger = logger;

    public async Task<Result<ProductResponse>> CreateProduct
        (ProductRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for product code and color duplication");
        if (await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Code == request.Code && x.Color == request.Color, cancellationToken))
            return Result.Failure<ProductResponse>(ProductErrors.CodeWithColorConflict(request.Code, request.Color));
        var product = new Product
        {
            Name = request.Name,
            Code = request.Code,
            Color = request.Color,
            Unit = request.Unit,
            Material = request.Material
        };

        await _appDbContext.Products.AddAsync(product, cancellationToken);
        _logger.LogInformation("product was added with id({id})", product.Id);
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
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<ProductWithInventoryResponse>(ProductErrors.NotFound);

        if(product.Inventory is null)
            return Result.Failure<ProductWithInventoryResponse>(ProductErrors.NoQuantity);

        var maxProductPurchase =
            await _appDbContext.Purchases.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .Where(x => x.PurchaseItems.Any(p => p.ProductID == id))
            .OrderByDescending(x => x.CreatedAt)
            .SelectMany(x => x.PurchaseItems)
            .Where(x => x.ProductID == id)
            .OrderByDescending(x => x.UnitCost)
            .FirstOrDefaultAsync();

        if(maxProductPurchase is null)
            return Result.Failure<ProductWithInventoryResponse>(ProductErrors.NotFound);

        return Result.Success(product.ToProductWithInventoryResponse(maxProductPurchase.UnitCost));
    }

    public async Task<Result<PaginatedList<ProductResponse>>> GetProducts
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Products.AsNoTracking();

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.CreatedAt >= utcFrom && x.CreatedAt <= utcTo);
        }

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.ProductResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(ProductSorts.ProductResponseSort(sortRequest));
        else
            query = query.OrderByDescending(ProductSorts.ProductResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToProductResponse());

        var response = await PaginatedList<ProductResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<StockTransactionResponse>>> GetProductStockTransactions
        (Guid id, PaginationRequest paginationRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<StockTransactionResponse>>(ProductErrors.NotFound);

        var query = _appDbContext.StockTransactions.AsNoTracking()
            .Where(x => x.ProductID == id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.ToStockTransactionResponse());

        var response = await PaginatedList<StockTransactionResponse>.CreateAsync
            (query, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<SaleResponse>>> GetSalesByProduct
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<SaleResponse>>(ProductErrors.NotFound);

        var query = _appDbContext.Products.AsNoTracking()
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.Sale)
            .SelectMany(x => x.SaleItems)
            .Select(x => x.Sale);

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.CreatedAt >= utcFrom && x.CreatedAt <= utcTo);
        }

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.SaleResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(SaleSorts.SaleResponseSort(sortRequest));
        else
            query = query.OrderByDescending(SaleSorts.SaleResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToSaleResponseWithNoItems());

        var response = await PaginatedList<SaleResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchasesByProduct
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<PurchaseResponse>>(ProductErrors.NotFound);

        var query = _appDbContext.Products.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Purchase)
            .SelectMany(x => x.PurchaseItems)
            .Select(x => x.Purchase);

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.CreatedAt >= utcFrom && x.CreatedAt <= utcTo);
        }

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.PurchaseResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(PurchaseSorts.PurchaseResponseSort(sortRequest));
        else
            query = query.OrderByDescending(PurchaseSorts.PurchaseResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToPurchaseResponseWithoutItems());

        var response = await PaginatedList<PurchaseResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<List<ProductWithInventoryResponse>>> GetProductsForBill
        (SearchProductBillByCodeRequest searchCode, CancellationToken cancellationToken = default)
    {
        var products = _appDbContext.Products.AsNoTracking()
            .Include(x => x.Inventory)
            .Include(x => x.PurchaseItems)
            .Where(x =>EF.Functions.Like(x.Code + "-" + x.Color, $"%{searchCode.code}%"))
            .Select(x => x.ToProductWithInventoryResponse(
                x.PurchaseItems
                    .Max(p => p.UnitCost)
            ));
        return Result.Success(await products.ToListAsync(cancellationToken));
    }

}
