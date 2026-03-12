namespace FabricesStoreManagementSystem.Implementations;

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
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInCatalog);
        }

        _logger.LogInformation("check product existance");
        var tempProducts = await _appDbContext.Products.AsNoTracking()
            .Where(x => request.Items.Select(y => y.ProductID).Contains(x.Id))
            .Select(x => new { x.Id, x.Code })
            .ToListAsync(cancellationToken);

        var checkIDs = tempProducts.Count == request.Items.Count;
        if (!checkIDs)
        {
            _logger.LogError("product or more not found");
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);
        }
        var checkCodes = tempProducts.DistinctBy(x => x.Code).Count() != 1;
        _logger.LogInformation("check code duplication");
        if (checkCodes)
        {
            _logger.LogError("there is one or more product code duplication");
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        }

        var catalog = new Catalog
        {
            CatalogCode = $"{tempProducts.First().Code}",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        await _appDbContext.Catalogs.AddAsync(catalog);

        _logger.LogInformation("start cutting product process");
        var results = new List<Result<string>>();
        foreach (var item in request.Items)
        {
            var result = await CutProduct(catalog.Id, item, cancellationToken);
            results.Add(result);
            if (result.IsFailure)
            {
                _logger.LogError("occure error through cutting process");
                return Result.Failure<CatalogResponse>(result.Error);
            }
        }
        _logger.LogInformation("end cutting product process");

        catalog.CatalogCode = results.First().Value;
        _logger.LogInformation("add catalog({id}) done", catalog.Id);
        return Result.Success(catalog.ToCatalogResponse());
    }

    private async Task<Result<string>> CutProduct
        (Guid catalogID, CatalogProductRequest product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("cutting product({productid}) for catalog({catalogid}) ", product.ProductID, catalogID);
        var productDetails = (await _appDbContext.Products.AsNoTracking()
            .Select(x => new { Id = x.Id, Code = x.Code })
            .SingleOrDefaultAsync(x => x.Id == product.ProductID, cancellationToken));
        _logger.LogInformation("check product existance");
        if (productDetails is null)
        {
            _logger.LogError("product({id}) not found", product.ProductID);
            return Result.Failure<string>(ProductErrors.NotFound);
        }

        var productInventory = await _appDbContext.Inventory.SingleOrDefaultAsync(x => x.ProductID == product.ProductID, cancellationToken);
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
            QuantityChange = -product.Quantity,
            TransactionType = StockTransactionType.Sample,
            ReferenceID = catalogID,
            ReferenceType = ReferenceTypes.Sample
        };

        var catalogProduct = new CatalogProduct
        {
            CatalogID = catalogID,
            ProductID = product.ProductID,
            Quantity = product.Quantity,
        };

        await _appDbContext.CatalogsProducts.AddAsync(catalogProduct, cancellationToken);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        _appDbContext.Inventory.Update(productInventory);
        _logger.LogInformation("add catalog product({productid}), and add stock transaction({transactionid}), update inventory", product.ProductID, stockTransaction.Id);
        return Result.Success(productDetails.Code);
    }

    public async Task<Result<CatalogResponse>> CreateCatalog
        (CatalogFromSupplierRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check products duplication");
        if (request.Items.Count != request.Items.DistinctBy(x => x).Count())
        {
            _logger.LogError("there is duplication product");
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInCatalog);
        }

        _logger.LogInformation("check supplier({id}) existance", request.SupplierID);
        if (!(await _appDbContext.Suppliers.AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
        {
            _logger.LogError("supplier({id}) not found", request.SupplierID);
            return Result.Failure<CatalogResponse>(SupplierErrors.NotFound);
        }

        _logger.LogInformation("check products existance");
        var existingIds = await _appDbContext.Products
            .AsNoTracking()
            .Where(x => request.Items.Contains(x.Id))
            .Select(x => new {x.Id, x.Code})
            .ToListAsync(cancellationToken);
        if (existingIds.Count != request.Items.Count)
        {
            _logger.LogError("product or more not found");
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);
        }

        var checkCodes = existingIds.DistinctBy(x => x.Code).Count();
        _logger.LogInformation("check code duplication");
        if (checkCodes != 1)
        {
            _logger.LogError("there is one or more code duplicated");
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        }

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
        };

        _logger.LogInformation("start cutting process");
        List<Result<string>> results = new List<Result<string>>();
        foreach (var item in request.Items)
        {
            var result = await CreateCatalogProductBySupplier(catalog.Id, item, cancellationToken);
            results.Add(result);
            if (result.IsFailure)
            {
                _logger.LogError("occure error through cutting process");
                return Result.Failure<CatalogResponse>(result.Error);
            }
        }
        _logger.LogInformation("end cutting process");

        catalog.CatalogCode = results.First().Value;

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
            ProductID = productID,
            Quantity = 0,
            IsDeducted = false,
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

        if (catalog.IsPurchased)
        {
            _logger.LogError("remove payment for purchased catalog");
            _appDbContext.Payments.RemoveRange(
                await _appDbContext.Payments.Where(x => x.ReferenceID == id).ToListAsync(cancellationToken)
            );
        }

        _appDbContext.Catalogs.Remove(
            (await _appDbContext.Catalogs.FindAsync(id, cancellationToken))!
        );

        _logger.LogInformation("remove catalog({id}) done", catalog.Id);
        return result;
    }

    private async Task<Result> RemoveSupplierCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("remove products from catalog({id})", catalogID);
        _appDbContext.CatalogsProducts.RemoveRange(
            await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID).ToListAsync(cancellationToken)
        );

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
        List<Result> results = new List<Result>();
        foreach (var item in catalogProducts)
        {
            var result = await ReturnProductInventory(item.ProductID, item.Quantity, cancellationToken);
            if (result.IsFailure)
                return Result.Failure(result.Error);
            results.Add(result);
        }
        _logger.LogInformation("end returning quantity to stock");

        _logger.LogInformation("remove stock transaction");
        _appDbContext.StockTransactions.RemoveRange(
            await _appDbContext.StockTransactions.Where(x => x.ReferenceID == catalogID && x.ReferenceType == ReferenceTypes.Sample).ToListAsync(cancellationToken)
        );

        _logger.LogInformation("remove products from catalog");
        _appDbContext.CatalogsProducts.RemoveRange(
            await _appDbContext.CatalogsProducts.Where(x => x.CatalogID == catalogID).ToListAsync(cancellationToken)
        );

        _logger.LogInformation("remove assigning rows for catalog");
        _appDbContext.CatalogsAssigns.RemoveRange(
            await _appDbContext.CatalogsAssigns.Where(x => x.CatalogID == catalogID).ToListAsync(cancellationToken)
        );

        _logger.LogInformation("remove catalog({id}) done", catalogID);
        return Result.Success();
    }

    private async Task<Result> ReturnProductInventory
        (Guid productID, decimal quantity, CancellationToken cancellationToken = default)
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
            CustomerID = request.CustomerID,
        };

        catalog.LastUpdateAt = DateTime.UtcNow;
        catalog.Status = CatalogStatus.Assigned;

        await _appDbContext.CatalogsAssigns.AddAsync(catalogAssign, cancellationToken);
        _logger.LogInformation("update catalog({id}) status, add assign({id}) catalog", request.CatalogID, catalogAssign.Id);
        return Result.Success(catalogAssign.ToAssignCatalogResponse());
    }

    public async Task<Result<AssignCatalogResponse>> ReturnCatalog
        (Guid catalogID, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check catalog({id}) if assign", catalogID);
        var catalogAssign = await _appDbContext.CatalogsAssigns
            .Include(x => x.Catalog)
            .Include(x => x.Customer)
            .SingleOrDefaultAsync(x => x.CatalogID == catalogID && !x.ReturnedAt.HasValue, cancellationToken);

        if (catalogAssign is null)
        {
            _logger.LogError("catalog({id}) not found assigned", catalogID);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotFoundAssignedCatalog);
        }

        _logger.LogInformation("check assign({id}) catalog status if assigned", catalogAssign.Id);
        if (catalogAssign.Catalog.Status != CatalogStatus.Assigned || catalogAssign.ReturnedAt is not null)
        {
            _logger.LogError("assign({id}) catalog not found", catalogAssign.Id);
            return Result.Failure<AssignCatalogResponse>(CatalogErrors.NotAssignedCatalog);
        }

        catalogAssign.ReturnedAt = DateTime.UtcNow;
        catalogAssign.Catalog.Status = CatalogStatus.Available;

        _appDbContext.Catalogs.Update(catalogAssign.Catalog);
        _appDbContext.CatalogsAssigns.Update(catalogAssign);
        _logger.LogInformation("update assign({assignid}) catalog return date, catalog({catalogid}) status", catalogAssign.Id, catalogAssign.CatalogID);
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
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Catalogs.AsNoTracking()
            .Include(x => x.Supplier)
            .AsQueryable();

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
            query = query.CatalogResponseSearch(searchRequest);

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
            .Include(x => x.Supplier)
            .Include(x => x.CatalogsProducts)
            .ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (catalog is null)
            return Result.Failure<CatalogResponse>(CatalogErrors.NotFound);

        return Result.Success(catalog.ToCatalogResponseWithItems());
    }

    public async Task<Result<CatalogResponse>> PurchaseCatalog
        (CatalogFormPurchaseCatalogRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check products duplication");
        if (request.Items.Count != request.Items.DistinctBy(x => x).Count())
        {
            _logger.LogError("there is duplication product");
            return Result.Failure<CatalogResponse>(ProductErrors.DuplicatedInCatalog);
        }

        _logger.LogInformation("check supplier({id}) existance", request.SupplierID);
        if (!(await _appDbContext.Suppliers.AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
        {
            _logger.LogError("supplier({id}) not found", request.SupplierID);
            return Result.Failure<CatalogResponse>(SupplierErrors.NotFound);
        }

        _logger.LogInformation("check products existance");
        var existingIds = await _appDbContext.Products
            .AsNoTracking()
            .Where(x => request.Items.Contains(x.Id))
            .Select(x => new { x.Id, x.Code })
            .ToListAsync(cancellationToken);
        if (existingIds.Count != request.Items.Count)
        {
            _logger.LogError("product or more not found");
            return Result.Failure<CatalogResponse>(ProductErrors.NotFoundID);
        }

        var checkCodes = existingIds.DistinctBy(x => x.Code).Count();
        _logger.LogInformation("check code duplication");
        if (checkCodes != 1)
        {
            _logger.LogError("there is one or more code duplicated");
            return Result.Failure<CatalogResponse>(CatalogErrors.ProductsNotSameCode);
        }

        var catalog = new Catalog
        {
            CatalogCode = "",
            Description = request.Description,
            Status = CatalogStatus.Available,
            ProductsCount = request.Items.Count,
            IsPurchased = true,
            SupplierID = request.SupplierID,
            Price = request.Amount,
            PaidAmount = request.PaidAmount
        };

        _logger.LogInformation("start cutting process");
        List<Result<string>> results = new List<Result<string>>();
        foreach (var item in request.Items)
        {
            var result = await CreateCatalogProductBySupplier(catalog.Id, item, cancellationToken);
            results.Add(result);
            if (result.IsFailure)
            {
                _logger.LogError("occure error through cutting process");
                return Result.Failure<CatalogResponse>(result.Error);
            }
        }
        _logger.LogInformation("end cutting process");
        catalog.CatalogCode = results.First().Value;
        if (request.PaidAmount > request.Amount)
        {
            _logger.LogError("paid more than amount");
            return Result.Failure<CatalogResponse>(CatalogErrors.PaidMoreThanAmount);
        }

        if (request.PaidAmount > 0)
        {
            var payment = new Payment
            {
                Amount = request.PaidAmount,
                ReferenceID = catalog.Id,
                ReferenceType = ReferenceTypes.Sample,
                PayMethod = PaymentMethod.Cash,
            };
            _logger.LogInformation("add purchase catalog payment");
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }

        await _appDbContext.Catalogs.AddAsync(catalog);
        _logger.LogInformation("Puchase catalog");
        return Result.Success(catalog.ToCatalogResponse());
    }

    public async Task<Result> PayForCatalog
        (Guid id, PurchaseUpdatePaidRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check purchase({id}) existance", id);
        if (await _appDbContext.Catalogs.SingleOrDefaultAsync
            (x => x.Id == id && x.IsPurchased, cancellationToken) is not { } catalogPurchase)
        {
            _logger.LogError("not found purchasee catalog({id})", id);
            return Result.Failure(CatalogErrors.NotFound);
        }

        _logger.LogInformation("check purchase catalog({id}) status if already 'Paid'", id);
        if (catalogPurchase.IsPaid is not null && (bool)catalogPurchase.IsPaid)
        {
            _logger.LogError("purchased catalog({id}) status is already paid", id);
            return Result.Failure(CatalogErrors.AlreadyPaid);
        }

        var paid = request.PaidAmount + catalogPurchase.PaidAmount;
        _logger.LogInformation("check purchase catalog({id}) paid if greater than total amount", id);
        if (paid > catalogPurchase.Price)
        {
            _logger.LogError("paid amount is greater than purchase catalog({id}) total amount", id);
            return Result.Failure(PurchaseErrors.PaidMoreThanTotal);
        }

        var payment = new Payment
        {
            ReferenceID = catalogPurchase.Id,
            ReferenceType = ReferenceTypes.Purchase,
            PayMethod = PaymentMethod.Cash,
            Amount = request.PaidAmount
        };
        _logger.LogInformation("add payment({id}) for purchased catalog({id})", payment.Id, id);
        await _appDbContext.Payments.AddAsync(payment, cancellationToken);

        catalogPurchase.PaidAmount = paid;
        catalogPurchase.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Catalogs.Update(catalogPurchase);
        _logger.LogInformation("execute update purchased catalog({id}) done", id);
        return Result.Success();
    }

    public async Task<Result<PaginatedList<AssignCatalogResponse>>> GetAssingedCatalogs
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest dateRangeRequest, SearchRequest searchRequest, bool includeReturned = false, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.CatalogsAssigns.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Catalog)
            .AsQueryable();

        if (!includeReturned)
            query = query.Where(x => !x.ReturnedAt.HasValue);

        if (dateRangeRequest is not null && dateRangeRequest.From is not null && dateRangeRequest.To is not null)
        {
            var timezone = !string.IsNullOrEmpty(dateRangeRequest.Timezone)
                ? dateRangeRequest.Timezone
                : "Arab Standard Time";
            var (utcFrom, utcTo) = DateRangeHelper.ConvertToUtcRange(
                dateRangeRequest.From.Value,
                dateRangeRequest.To.Value,
                timezone);
            query = query.Where(x => x.AssignedAt >= utcFrom && x.AssignedAt <= utcTo);
        }

        if (searchRequest is not null && searchRequest.Search is not null)
            query = query.AssignCatalogResponseSearch(searchRequest);

        if (sortRequest.SortDir?.ToLower() == "asc" || sortRequest.SortDir?.ToLower() == "ascending")
            query = query.OrderBy(AssignCatalogSorts.AssignCatalogResponseSort(sortRequest));
        else
            query = query.OrderByDescending(AssignCatalogSorts.AssignCatalogResponseSort(sortRequest));

        var result = query
            .Select(x => x.ToAssignCatalogResponse());

        var response = await PaginatedList<AssignCatalogResponse>.CreateAsync
            (result, paginationRequest.Page, paginationRequest.PageSize, cancellationToken);

        return Result.Success(response);
    }
}
