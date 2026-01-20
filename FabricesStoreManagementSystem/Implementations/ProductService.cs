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
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (product is null)
            return Result.Failure<ProductWithInventoryResponse>(ProductErrors.NotFound);

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
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Products.AsNoTracking();

        if (dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                            DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(ProductSorts.ProductResponseSort(sortRequest));
        else
            query = query.OrderBy(ProductSorts.ProductResponseSort(sortRequest));

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
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<SaleResponse>>(ProductErrors.NotFound);

        var query = _appDbContext.Products.AsNoTracking()
            .Include(x => x.SaleItems)
            .ThenInclude(x => x.Sale)
            .SelectMany(x => x.SaleItems)
            .Select(x => x.Sale);

        if (dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                            DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(SaleSorts.SaleResponseSort(sortRequest));
        else
            query = query.OrderBy(SaleSorts.SaleResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToSaleResponseWithNoItems());

        var response = await PaginatedList<SaleResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchasesByProduct
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken)))
            return Result.Failure<PaginatedList<PurchaseResponse>>(ProductErrors.NotFound);

        var query = _appDbContext.Products.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .ThenInclude(x => x.Purchase)
            .SelectMany(x => x.PurchaseItems)
            .Select(x => x.Purchase);

        if(dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                            DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(PurchaseSorts.PurchaseResponseSort(sortRequest));
        else
            query = query.OrderBy(PurchaseSorts.PurchaseResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToPurchaseResponseWithoutItems());

        var response = await PaginatedList<PurchaseResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result> CutSampleForCatalog
        (CutCatalogRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Product.Count != request.Product.DistinctBy(x => x.Id).Count())
            return Result.Failure(ProductErrors.DuplicatedInInvoice);

        var checkIDs = await _appDbContext.Products.AsNoTracking()
            .AnyAsync(x => request.Product.Select(i => i.Id).Contains(x.Id));

        if (!checkIDs)
            return Result.Failure(ProductErrors.NotFoundID);

        var cutProcesses = request.Product.Select(x => CutProduct(x, cancellationToken));
        var resultCutting = Task.WhenAll(cutProcesses).Result;
        foreach (var res in resultCutting)
            if (res.IsFailure)
                return Result.Failure(res.Error);

        return Result.Success();
    }

    private async Task<Result> CutProduct
        (ProductCatalogRequest product, CancellationToken cancellationToken = default)
    {
        var productInventory = await _appDbContext.Inventory.FindAsync(product.Id, cancellationToken);
        if (productInventory is null)
            return Result.Failure(ProductErrors.NoQuantity);

        if (productInventory.CurrentQuantity < product.Quantity)
            return Result.Failure(ProductErrors.NoEnoughQuantity);

        productInventory.CurrentQuantity -= product.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        var stockTransaction = new StockTransaction
        {
            ProductID = product.Id,
            Note = "قص من اجل الكاتالوغ",
            QuantityChange = -1f * product.Quantity,
            TransactionType = StockTransactionType.Sample            
        };

        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        _appDbContext.Inventory.Update(productInventory);
        return Result.Success();
    }

}
