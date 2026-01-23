namespace FabricesStoreManagementSystem.Implementations;

public class CatalogService(AppDbContext appDbContext) : ICatalogService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<CatalogResponse>> CreateCatalog
        (CatalogRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count != request.Items.DistinctBy(x => x.ProductID).Count())
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInInvoice);

        var checkIDs = await _appDbContext.Products.AsNoTracking()
            .AnyAsync(x => request.Items.Select(i => i.ProductID).Contains(x.Id));

        if (!checkIDs)
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        var cutProcesses = request.Items.Select(x => CutProduct(catalog.Id, x, cancellationToken));
        var resultCutting = Task.WhenAll(cutProcesses).Result;
        foreach (var res in resultCutting)
            if (res.IsFailure)
                return Result.Failure<CatalogResponse>(res.Error);

        if (resultCutting.Select(x => x.Value).Distinct().Count() != resultCutting.Select(x => x.Value).Count())
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        
        catalog.CatalogCode = resultCutting.First().Value;

        await _appDbContext.Catalogs.AddAsync(catalog);

        return Result.Success(catalog.ToCatalogResponse());
    }

    private async Task<Result<string>> CutProduct
        (Guid catalogID, CatalogProductRequest product, CancellationToken cancellationToken = default)
    {
        var productDetails = (await _appDbContext.Products.AsNoTracking()
            .Select(x => new { Id = x.Id, Code = x.Code })
            .SingleOrDefaultAsync(x => x.Id == product.ProductID, cancellationToken));
        if (productDetails is null)
            return Result.Failure<string>(ProductErrors.NotFound);

        var productInventory = await _appDbContext.Inventory.FindAsync(product.ProductID, cancellationToken);
        if (productInventory is null)
            return Result.Failure<string>(ProductErrors.NoQuantity);

        if (productInventory.CurrentQuantity < product.Quantity)
            return Result.Failure<string>(ProductErrors.NoEnoughQuantity);

        productInventory.CurrentQuantity -= product.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        var stockTransaction = new StockTransaction
        {
            ProductID = product.ProductID,
            Note = "قص من اجل الكاتالوغ",
            QuantityChange = -1f * product.Quantity,
            TransactionType = StockTransactionType.Sample,
            ReferenceID = catalogID,
            ReferenceType = ReferenceTypes.Sample
        };

        var catalogProduct = new CatalogProduct
        {
            CatalogID = catalogID,
            PorductID = product.ProductID,
            Quantity = product.Quantity,
        };

        await _appDbContext.CatalogsProducts.AddAsync(catalogProduct, cancellationToken);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        _appDbContext.Inventory.Update(productInventory);

        return Result.Success(productDetails.Code);
    }

    public async Task<Result<CatalogResponse>> CreateCatalog
        (CatalogFromSupplierRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Suppliers.AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
            return Result.Failure<CatalogResponse>(SupplierErrors.NotFound);

        if (request.Items.Count != request.Items.DistinctBy(x => x).Count())
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInInvoice);

        var checkIDs = await _appDbContext.Products.AsNoTracking()
            .AnyAsync(x => request.Items.Contains(x.Id));

        if (!checkIDs)
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        var cutProcesses = request.Items.Select(x => CreateCatalogProductBySupplier(catalog.Id, x, cancellationToken));
        var resultCutting = Task.WhenAll(cutProcesses).Result;
        foreach (var res in resultCutting)
            if (res.IsFailure)
                return Result.Failure<CatalogResponse>(res.Error);

        if (resultCutting.Select(x => x.Value).Distinct().Count() != resultCutting.Select(x => x.Value).Count())
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);

        catalog.CatalogCode = resultCutting.First().Value;

        await _appDbContext.Catalogs.AddAsync(catalog);

        return Result.Success(catalog.ToCatalogResponse());
    }

    private async Task<Result<string>> CreateCatalogProductBySupplier
        (Guid catalogID, Guid productID, CancellationToken cancellationToken = default)
    {
        var productDetails = (await _appDbContext.Products.AsNoTracking()
            .Select(x => new { Id = x.Id, Code = x.Code })
            .SingleOrDefaultAsync(x => x.Id == productID, cancellationToken));
        if (productDetails is null)
            return Result.Failure<string>(ProductErrors.NotFound);

        var catalogProduct = new CatalogProduct
        {
            CatalogID = catalogID,
            PorductID = productID,
            Quantity = 0,
            IsDeducted = false
        };

        await _appDbContext.CatalogsProducts.AddAsync(catalogProduct, cancellationToken);
        return Result.Success(productDetails.Code);
    }

    public async Task<Result> RemoveCatalog
        (Guid id, CancellationToken cancellationToken = default)
    {
        var catalog = await _appDbContext.Catalogs
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (catalog is null)
            return Result.Failure(CatalogErrors.NotFound);

        if (catalog.Status != CatalogStatus.Available)
            return Result.Failure(CatalogErrors.UnableToProcessCatalogWhichUnavailable);

        Result? result = null;
        if (catalog.SupplierID is not null)
            result = await RemoveSupplierCatalog(id, cancellationToken);
        else
            result = await RemoveStockCatalog(id, cancellationToken);

        if (result is null)
            return Result.Failure(GeneralErrors.UnexpectedError);

        await _appDbContext.Catalogs.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return result;
    }

    private async Task<Result> RemoveSupplierCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> RemoveStockCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        var catalogProducts = await _appDbContext.CatalogsProducts
            .Where(x => x.CatalogID == catalogID)
            .ToListAsync(cancellationToken);

        var returnProductStockProcess = catalogProducts.Select(x => ReturnProductInventory(x.PorductID, x.Quantity, cancellationToken));
        var resultProductStockProcess = await Task.WhenAll(returnProductStockProcess);
        foreach (var productProcess in resultProductStockProcess)
            if(productProcess.IsFailure)
                return Result.Failure(productProcess.Error);

        await _appDbContext.StockTransactions
            .Where(x => x.ReferenceID == catalogID && x.ReferenceType == ReferenceTypes.Sample)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.CatalogsAssigns.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> ReturnProductInventory
        (Guid productID, float quantity, CancellationToken cancellationToken = default)
    {
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID, cancellationToken);

        if (productInventory is null)
            return Result.Failure(ProductErrors.NoQuantity);

        productInventory.CurrentQuantity += quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _appDbContext.Inventory.Update(productInventory);

        return Result.Success();
    }

    public async Task<Result<AssignCatalogResponse>> AssignCatalog
        (AssignCatalogRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == request.CustomerID && x.IsActive, cancellationToken)))
            return Result.Failure<AssignCatalogResponse>(CustomerErrors.NotFound);

        var catalog = await _appDbContext.Catalogs.FindAsync(request.CatalogID, cancellationToken);
        if (catalog is null)
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotFound);

        if (catalog.Status != CatalogStatus.Available)
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.UnavailableCatalog);

        var catalogAssign = new CatalogAssign
        {
            CatalogID = request.CatalogID,
            CustomerID = request.CatalogID,
        };

        catalog.LastUpdateAt = DateTime.UtcNow;

        await _appDbContext.CatalogsAssigns.AddAsync(catalogAssign, cancellationToken);
        _appDbContext.Catalogs.Update(catalog);

        return Result.Success(catalogAssign.ToAssignCatalogResponse());
    }

    public async Task<Result<AssignCatalogResponse>> ReturnCatalog
        (Guid assignID, CancellationToken cancellationToken = default)
    {
        var catalogAssign = await _appDbContext.CatalogsAssigns
            .Include(x => x.Catalog)
            .SingleOrDefaultAsync(x => x.Id == assignID, cancellationToken);

        if (catalogAssign is null)
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotFoundAssignedCatalog);

        if (catalogAssign.Catalog.Status != CatalogStatus.Assigned || catalogAssign.ReturnedAt is not null)
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotAssignedCatalog);

        catalogAssign.ReturnedAt = DateTime.UtcNow;
        catalogAssign.Catalog.Status = CatalogStatus.Available;

        _appDbContext.Catalogs.Update(catalogAssign.Catalog);
        _appDbContext.CatalogsAssigns.Update(catalogAssign);

        return Result.Success(catalogAssign.ToAssignCatalogResponse());
    }

    public async Task<Result> DestructionCatalog
        (Guid id, CancellationToken cancellationToken = default)
    {
        var catalog = await _appDbContext.Catalogs.FindAsync(id, cancellationToken);

        if (catalog is null)
            return Result.Failure(CatalogErrors.NotFound);

        if (catalog.Status == CatalogStatus.Lost)
            return Result.Failure(CatalogErrors.CatalogAlreadyLost);

        catalog.Status = CatalogStatus.Lost;
        catalog.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Catalogs.Update(catalog);

        return Result.Success();
    }

    public async Task<Result<PaginatedList<CatalogResponse>>> GetCatalogs
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Catalogs.AsNoTracking();

        if(dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                            DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To);

        if (sortRequest.SortDir?.ToLower() == "asc")
            query = query.OrderByDescending(CatalogSorts.CatalogResponseSort(sortRequest));
        else
            query = query.OrderBy(CatalogSorts.CatalogResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToCatalogResponse());

        var response = await PaginatedList<CatalogResponse>.CreateAsync
         (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<CatalogResponse>> GetCatalog
        (Guid id, CancellationToken cancellationToken = default)
    {
        var catalog = await _appDbContext.Catalogs.AsNoTracking()
            .Include(x => x.CatalogsProducts)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (catalog is null)
            return Result.Failure<CatalogResponse>(CatalogErrors.NotFound);

        return Result.Success(catalog.ToCatalogResponse());
    }
}
