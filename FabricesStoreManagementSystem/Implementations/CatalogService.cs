namespace FabricesStoreManagmentSystem.Implementations;

public class CatalogService(AppDbContext appDbContext, ILogger<CatalogService> logger) : ICatalogService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<CatalogService> _logger = logger;

    public async Task<Result<CatalogResponse>> CreateCatalog
        (CatalogRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check product duplication");
        if (request.Items.Count != request.Items.DistinctBy(x => x.ProductID).Count())
        {
            _logger.LogError("there is duplcated product");
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInInvoice);
        }

        _logger.LogInformation("check product existance");
        var checkIDs = await _appDbContext.Products.AsNoTracking()
            .AnyAsync(x => request.Items.Select(i => i.ProductID).Contains(x.Id));

        if (!checkIDs)
        {
            _logger.LogError("product or more not found");
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);
        }

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        _logger.LogInformation("start cutting product process");
        var cutProcesses = request.Items.Select(x => CutProduct(catalog.Id, x, cancellationToken));
        var resultCutting = Task.WhenAll(cutProcesses).Result;
        _logger.LogInformation("end cutting product process");
        foreach (var res in resultCutting)
            if (res.IsFailure)
            {
                _logger.LogError("occure error through cutting process");
                return Result.Failure<CatalogResponse>(res.Error);
            }

        _logger.LogInformation("check code duplication");
        if (resultCutting.Select(x => x.Value).Distinct().Count() != resultCutting.Select(x => x.Value).Count())
        {
            _logger.LogError("there is one or more product code duplication");
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        }
        
        catalog.CatalogCode = resultCutting.First().Value;

        await _appDbContext.Catalogs.AddAsync(catalog);
        _logger.LogInformation("add catalog({id}) done", catalog.Id);
        return Result.Success(catalog.ToCatalogResponse());
    }

    private async Task<Result<string>> CutProduct
        (Guid catalogID, CatalogProductRequest product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("cutting product({id}) for catalog({id}) ", product.ProductID, catalogID);
        var productDetails = (await _appDbContext.Products.AsNoTracking()
            .Select(x => new { Id = x.Id, Code = x.Code })
            .SingleOrDefaultAsync(x => x.Id == product.ProductID, cancellationToken));
        _logger.LogInformation("check product existance");
        if (productDetails is null)
        {
            _logger.LogError("product({id}) not found", product.ProductID);
            return Result.Failure<string>(ProductErrors.NotFound);
        }

        var productInventory = await _appDbContext.Inventory.FindAsync(product.ProductID, cancellationToken);
        _logger.LogInformation("check product inventory existance");
        if (productInventory is null)
        {
            _logger.LogError("product({id}) inventory not found", product.ProductID);
            return Result.Failure<string>(ProductErrors.NoQuantity);
        }

        _logger.LogInformation("check current stock quantity if it is enough to cutting");
        if (productInventory.CurrentQuantity < product.Quantity)
        {
            _logger.LogError("no enough quantity for cutting");
            return Result.Failure<string>(ProductErrors.NoEnoughQuantity);
        }

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

        _logger.LogInformation("add catalog product({id}), and add stock transaction({id}), update inventory", product.ProductID, stockTransaction.Id);
        return Result.Success(productDetails.Code);
    }

    public async Task<Result<CatalogResponse>> CreateCatalog
        (CatalogFromSupplierRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check products duplication");
        if (request.Items.Count != request.Items.DistinctBy(x => x).Count())
        {
            _logger.LogError("there is duplication product");
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInInvoice);
        }

        _logger.LogInformation("check supplier({id}) existance", request.SupplierID);
        if (!(await _appDbContext.Suppliers.AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
        {
            _logger.LogError("supplier({id}) not found", request.SupplierID);
            return Result.Failure<CatalogResponse>(SupplierErrors.NotFound);
        }

        _logger.LogInformation("check products existance");
        var checkIDs = await _appDbContext.Products.AsNoTracking()
            .AnyAsync(x => request.Items.Contains(x.Id));

        if (!checkIDs)
        {
            _logger.LogError("product or more not found");
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);
        }

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        _logger.LogInformation("start cutting process");
        var cutProcesses = request.Items.Select(x => CreateCatalogProductBySupplier(catalog.Id, x, cancellationToken));
        var resultCutting = Task.WhenAll(cutProcesses).Result;
        _logger.LogInformation("end cutting process");
        foreach (var res in resultCutting)
            if (res.IsFailure)
            {
                _logger.LogError("start cutting process");
                return Result.Failure<CatalogResponse>(res.Error);
            }

        _logger.LogInformation("check code duplication");
        if (resultCutting.Select(x => x.Value).Distinct().Count() != resultCutting.Select(x => x.Value).Count())
        {
            _logger.LogError("there is one or more code duplicated");
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        }

        catalog.CatalogCode = resultCutting.First().Value;

        await _appDbContext.Catalogs.AddAsync(catalog);
        _logger.LogInformation("add catalog");
        return Result.Success(catalog.ToCatalogResponse());
    }

    private async Task<Result<string>> CreateCatalogProductBySupplier
        (Guid catalogID, Guid productID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check product({id}) existance", productID);
        var productDetails = (await _appDbContext.Products.AsNoTracking()
            .Select(x => new { Id = x.Id, Code = x.Code })
            .SingleOrDefaultAsync(x => x.Id == productID, cancellationToken));
        if (productDetails is null)
        {
            _logger.LogError("product({id}) not found", productID);
            return Result.Failure<string>(ProductErrors.NotFound);
        }

        var catalogProduct = new CatalogProduct
        {
            CatalogID = catalogID,
            PorductID = productID,
            Quantity = 0,
            IsDeducted = false
        };

        await _appDbContext.CatalogsProducts.AddAsync(catalogProduct, cancellationToken);
        _logger.LogInformation("add product({id}) to catalog({id})", productID, catalogID);
        return Result.Success(productDetails.Code);
    }

    public async Task<Result> RemoveCatalog
        (Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check catalog({id}) existance", id);
        var catalog = await _appDbContext.Catalogs
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (catalog is null)
        {
            _logger.LogError("catalog({id}) not found", id);
            return Result.Failure(CatalogErrors.NotFound);
        }

        _logger.LogInformation("check catalog({id}) status if available", id);
        if (catalog.Status != CatalogStatus.Available)
        {
            _logger.LogError("the state of catalog({id}) not available", id);
            return Result.Failure(CatalogErrors.UnableToProcessCatalogWhichUnavailable);
        }

        Result? result = null;
        if (catalog.SupplierID is not null)
        {
            _logger.LogInformation("remove catalog by supplier({id})", catalog.SupplierID);
            result = await RemoveSupplierCatalog(id, cancellationToken);
        }
        else
        {
            _logger.LogInformation("remove catalog from stock");
            result = await RemoveStockCatalog(id, cancellationToken);
        }

        if (result is null)
        {
            _logger.LogInformation("unexpected error happen after try to remove catalog");
            return Result.Failure(GeneralErrors.UnexpectedError);
        }

        await _appDbContext.Catalogs.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        _logger.LogInformation("remove catalog({id}) done", catalog.Id);
        return result;
    }

    private async Task<Result> RemoveSupplierCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("remove products from catalog({id})", catalogID);
        await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> RemoveStockCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("remove products from stock");
        var catalogProducts = await _appDbContext.CatalogsProducts
            .Where(x => x.CatalogID == catalogID)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("start returning quantity to stock");
        var returnProductStockProcess = catalogProducts.Select(x => ReturnProductInventory(x.PorductID, x.Quantity, cancellationToken));
        var resultProductStockProcess = await Task.WhenAll(returnProductStockProcess);
        _logger.LogInformation("end returning quantity to stock");
        foreach (var productProcess in resultProductStockProcess)
            if (productProcess.IsFailure)
            {
                _logger.LogError("start returning quantity to stock");
                return Result.Failure(productProcess.Error);
            }

        _logger.LogInformation("remove stock transaction");
        await _appDbContext.StockTransactions
            .Where(x => x.ReferenceID == catalogID && x.ReferenceType == ReferenceTypes.Sample)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("remove products from catalog");
        await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("remove assigning rows for catalog");
        await _appDbContext.CatalogsAssigns.Where(x => x.CatalogID == catalogID)
            .ExecuteDeleteAsync(cancellationToken);

        _logger.LogInformation("remove catalog({id}) done", catalogID);
        return Result.Success();
    }

    private async Task<Result> ReturnProductInventory
        (Guid productID, float quantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check product inventory");
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID, cancellationToken);

        if (productInventory is null)
        {
            _logger.LogError("inventory not found");
            return Result.Failure(ProductErrors.NoQuantity);
        }

        productInventory.CurrentQuantity += quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _appDbContext.Inventory.Update(productInventory);
        _logger.LogInformation("return product({id}) inventory done", productID);
        return Result.Success();
    }

    public async Task<Result<AssignCatalogResponse>> AssignCatalog
        (AssignCatalogRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check customer({id}) existance", request.CustomerID);
        if (!(await _appDbContext.Customers.AsNoTracking().AnyAsync(x => x.Id == request.CustomerID && x.IsActive, cancellationToken)))
        {
            _logger.LogError("customer({id}) not found", request.CustomerID);
            return Result.Failure<AssignCatalogResponse>(CustomerErrors.NotFound);
        }

        _logger.LogInformation("check catalog({id}) existance", request.CatalogID);
        var catalog = await _appDbContext.Catalogs.FindAsync(request.CatalogID, cancellationToken);
        if (catalog is null)
        {
            _logger.LogError("catalog({id}) not found", request.CatalogID);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotFound);
        }

        _logger.LogInformation("catalog({id}) status availability", request.CatalogID);
        if (catalog.Status != CatalogStatus.Available)
        {
            _logger.LogError("catalog({id}) not available", request.CatalogID);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.UnavailableCatalog);
        }

        var catalogAssign = new CatalogAssign
        {
            CatalogID = request.CatalogID,
            CustomerID = request.CatalogID,
        };

        catalog.LastUpdateAt = DateTime.UtcNow;

        await _appDbContext.CatalogsAssigns.AddAsync(catalogAssign, cancellationToken);
        _appDbContext.Catalogs.Update(catalog);
        _logger.LogInformation("update catalog({id}) status, add assign({id}) catalog", request.CatalogID, catalogAssign.Id);
        return Result.Success(catalogAssign.ToAssignCatalogResponse());
    }

    public async Task<Result<AssignCatalogResponse>> ReturnCatalog
        (Guid assignID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check assign({id}) catalog", assignID);
        var catalogAssign = await _appDbContext.CatalogsAssigns
            .Include(x => x.Catalog)
            .SingleOrDefaultAsync(x => x.Id == assignID, cancellationToken);

        if (catalogAssign is null)
        {
            _logger.LogError("catalog assign({id}) not found", assignID);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotFoundAssignedCatalog);
        }

        _logger.LogInformation("check assign({id}) catalog status if assigned", assignID);
        if (catalogAssign.Catalog.Status != CatalogStatus.Assigned || catalogAssign.ReturnedAt is not null)
        {
            _logger.LogError("assign({id}) catalog not found", assignID);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotAssignedCatalog);
        }

        catalogAssign.ReturnedAt = DateTime.UtcNow;
        catalogAssign.Catalog.Status = CatalogStatus.Available;

        _appDbContext.Catalogs.Update(catalogAssign.Catalog);
        _appDbContext.CatalogsAssigns.Update(catalogAssign);
        _logger.LogInformation("update assign({id}) catalog return date, catalog({id}) status", assignID, catalogAssign.CatalogID);
        return Result.Success(catalogAssign.ToAssignCatalogResponse());
    }

    public async Task<Result> DestructionCatalog
        (Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check catalog({id}) existance", id);
        var catalog = await _appDbContext.Catalogs.FindAsync(id, cancellationToken);

        if (catalog is null)
        {
            _logger.LogError("catalog({id}) not found", id);
            return Result.Failure(CatalogErrors.NotFound);
        }

        _logger.LogInformation("check catalog({id}) status if not lost", id);
        if (catalog.Status == CatalogStatus.Lost)
        {
            _logger.LogError("catalog({id}) status is 'Lost'", id);
            return Result.Failure(CatalogErrors.CatalogAlreadyLost);
        }

        catalog.Status = CatalogStatus.Lost;
        catalog.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Catalogs.Update(catalog);
        _logger.LogInformation("update catalog({id}) status", id);
        return Result.Success();
    }

    public async Task<Result<PaginatedList<CatalogResponse>>> GetCatalogs
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, SearchRequest? searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Catalogs.AsNoTracking();

        if (dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRangeRequest.From &&
                            DateOnly.Parse(x.CreatedAt.ToString()) <= dateRangeRequest.To);

        if (searchRequest is not null)
            query = query
                .Where(x => CatalogSearchs.CatalogResponseSearch(searchRequest).ToString().ToLower().Contains(searchRequest.Search.ToLower()));

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(CatalogSorts.CatalogResponseSort(sortRequest));
        else
            query = query.OrderByDescending(CatalogSorts.CatalogResponseSort(sortRequest));

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
