namespace FabricesStoreManagementSystem.Implementations;

public class PurchaseService(AppDbContext appDbContext, IProductService productService, ILogger<PurchaseService> logger) : IPurchaseService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IProductService _productService = productService;
    private readonly ILogger<PurchaseService> _logger = logger;

    public async Task<Result<PurchaseResponse>> CreatePurchase
        (PurchaseRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check purchase product duplication");
        if (request.PurchaseItems.Count() != request.PurchaseItems.DistinctBy(x => x.ProductID).Count())
        {
            _logger.LogError("there is duplication in purchase items");
            return Result.Failure<PurchaseResponse>(ProductErrors.DuplicatedInInvoice);
        }

        _logger.LogInformation("check supplier existance");
        if (!(await _appDbContext.Suppliers.AsNoTracking()
            .AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
        {
            _logger.LogInformation("supplier({id}) not found or not active", request.SupplierID);
            return Result.Failure<PurchaseResponse>(SupplierErrors.NotFound);
        }

        var purchase = new Purchase
        {
            InvoiceNumber = HelperTools.GenerateInvoiceNumber(),
            SupplierID = request.SupplierID,
            ProductsCount = request.PurchaseItems.Count,
            Status = PayStatuses.NotPaid,
        };

        _logger.LogInformation("add purchase but process still running");
        await _appDbContext.Purchases.AddAsync(purchase, cancellationToken);

        _logger.LogInformation("start adding purchase items");
        var processPurchaseItems = await CreatePurchaseItems(purchase.Id, request.PurchaseItems, cancellationToken);
        if (processPurchaseItems.IsFailure)
        {
            _logger.LogError("one or more purchase items causes error");
            return Result.Failure<PurchaseResponse>(processPurchaseItems.Error);
        }
        var resultPurchaseItems = processPurchaseItems.Value;

        purchase.ProductsCount = resultPurchaseItems.Count;
        purchase.TotalAmount = resultPurchaseItems.Sum(x => x.Total);
        purchase.PaidAmount = request.PaidAmount;

        var payment = new Payment
        {
            ReferenceID = purchase.Id,
            ReferenceType = ReferenceTypes.Purchase,
            PayMethod = PaymentMethod.Cash,
            Amount = purchase.PaidAmount
        };

        _logger.LogInformation("check for paid amount if greater than total amount");
        if (purchase.PaidAmount > purchase.TotalAmount)
        {
            _logger.LogError("paid amount greater than total amount for purchase({id})", purchase.Id);
            return Result.Failure<PurchaseResponse>(PurchaseErrors.PaidMoreThanTotal);
        }
        else if (purchase.PaidAmount == purchase.TotalAmount)
        {
            _logger.LogInformation("update purchase({id}) status to 'Paid'", purchase.Id);
            purchase.Status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            _logger.LogError("add payment({id}) for purchase({id})", payment.Id, purchase.Id);
        }
        else if (purchase.PaidAmount  > 0 && purchase.PaidAmount < purchase.TotalAmount)
        {
            _logger.LogInformation("update purchase({id}) status to 'NotCompleted'", purchase.Id);
            purchase.Status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            _logger.LogError("add payment({id}) for purchase({id})", payment.Id, purchase.Id);
        }
        else
        {
            _logger.LogInformation("update purchase({id}) status to 'NotPaid'", purchase.Id);
            purchase.Status = PayStatuses.NotPaid;
        }

        _logger.LogInformation("create purchase({id}) done", purchase.Id);
        return Result.Success(purchase.ToPurchaseResponseWithoutItems());
    }

    private async Task<Result<List<PurchaseItem>>> CreatePurchaseItems
        (Guid id, List<PurchaseItemRequest> items, CancellationToken cancellationToken)
    {
        _logger.LogInformation("start adding purchase({id}) items", id);
        var processItems = items
            .Select(x => new PurchaseItem
            {
                ProductID = x.ProductID,
                Quantity = x.Quantity,
                UnitCost = x.UnitCost,
                PurchaseID = id,
            })
            .Select(x => CreatePurchaseItem(x, cancellationToken));
        var results = Task.WhenAll(processItems).Result;
        _logger.LogInformation("end add purchase({id}) items process", id);
        if (results is null || results.Length == 0)
        {
            _logger.LogError("one or more purchase item causes error");
            return Result.Failure<List<PurchaseItem>>(PurchaseErrors.NoSuccessfulPurchsaeItems);
        }

        foreach (var r in results)
            if (r.IsFailure)
            {
                _logger.LogError("one or more purchase item causes error");
                return Result.Failure<List<PurchaseItem>>(r.Error);
            }
        var res = results.Select(x => x.Value).ToList();
        _logger.LogError("add purchase({id}) items done", id);
        return Result.Success(res);
    }

    private async Task<Result<PurchaseItem>> CreatePurchaseItem
        (PurchaseItem purchaseItem, CancellationToken cancellationToken = default)
    {

        _logger.LogInformation("check product({id}) existance", purchaseItem.ProductID);
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == purchaseItem.ProductID, cancellationToken)))
        {
            _logger.LogError("product({id}) not found", purchaseItem.ProductID);
            return Result.Failure<PurchaseItem>(ProductErrors.NotFound);
        }

        Inventory productInventory = (await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == purchaseItem.ProductID, cancellationToken)) ??
             new Inventory
             {
                 CurrentQuantity = 0,
                 ProductID = purchaseItem.ProductID
             };

        var stockTransaction = new StockTransaction
        {
            Note = "تمت عملية شراء من قبل المدير وتم اضافة الكمية للمخزن",
            ProductID = purchaseItem.ProductID,
            QuantityChange = purchaseItem.Quantity,
            ReferenceID = purchaseItem.PurchaseID,
            ReferenceType = ReferenceTypes.Purchase,
            TransactionType = StockTransactionType.Purchase
        };

        _logger.LogInformation("add purchase({id}) items", purchaseItem.PurchaseID);
        await _appDbContext.PurchaseItems.AddAsync(purchaseItem, cancellationToken);
        _logger.LogInformation("add stock-transaction({id})", stockTransaction.Id);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        productInventory.CurrentQuantity += purchaseItem.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Inventory.Update(productInventory);

        _logger.LogInformation("create purchase({id}) done", purchaseItem.PurchaseID);
        return Result.Success(purchaseItem);
    }

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchases
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, SearchRequest? searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Purchases.AsNoTracking();

        if (dateRangeRequest is not null)
            query = query
                    .Where(x => DateOnly.FromDateTime(x.CreatedAt) >= dateRangeRequest.From &&
                                DateOnly.FromDateTime(x.CreatedAt) <= dateRangeRequest.To);

        if (searchRequest is not null)
            query = query
                .Where(x => PurchaseSearchs.PurchaseResponseSearch(searchRequest).ToString().ToLower().Contains(searchRequest.Search));

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

    public async Task<Result<PurchaseResponse>> GetPurchase
        (Guid id, CancellationToken cancellationToken = default)
    {
        var purchase = await _appDbContext.Purchases.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (purchase is null)
            return Result.Failure<PurchaseResponse>(PurchaseErrors.NotFound);

        return Result.Success(purchase.ToPurchaseResponse());
    }

    public async Task<Result<PurchaseResponse>> GetPurchaseByInvoiceNumber
        (string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var purchase = await _appDbContext.Purchases.AsNoTracking()
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber, cancellationToken);

        if (purchase is null)
            return Result.Failure<PurchaseResponse>(PurchaseErrors.NotFound);

        return Result.Success(purchase.ToPurchaseResponse());
    }

    public async Task<Result> RemovePurchase
        (Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check purchase({id}) existance", id);
        var purchase = await _appDbContext.Purchases
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (purchase is null)
        {
            _logger.LogError("purchase({id}) not found", id);
            return Result.Failure(PurchaseErrors.NotFound);
        }

        var getSaleProcesses = purchase.PurchaseItems
            .Select(x => _productService
                            .GetSalesByProduct(x.ProductID,
                                                new PaginationRequest(1, 1),
                                                new SortRequest(null, null),
                                                new DateRangeRequest(DateOnly.FromDateTime(purchase.CreatedAt), DateOnly.FromDateTime(DateTime.UtcNow)),
                                                null,
                                                cancellationToken));

        var salesResultProcess = await Task.WhenAll(getSaleProcesses);
        foreach (var item in salesResultProcess)
        {
            if (item.IsFailure)
            {
                _logger.LogInformation("purchase({id}) item or more causes error", id);
                return Result.Failure(item.Error);
            }
        }

        var check = salesResultProcess.Select(x => x.Value.TotalItems).Max();
        if (check > 0)
        {
            _logger.LogError("return purchase items faild");
            return Result.Failure(PurchaseErrors.UnableToReturnPurchase);
        }

        var returnProductQuantities = purchase.PurchaseItems
            .Select(x => returnQuantity(id, x.ProductID, x.Quantity, cancellationToken));

        var endedReturnQuantities = await Task.WhenAll(returnProductQuantities);
        foreach (var ended in endedReturnQuantities)
            if(ended.IsFailure)
                return Result.Failure(ended.Error);

        _logger.LogInformation("remove payment for purchase({id})", id);
        _appDbContext.Payments.RemoveRange(
            await _appDbContext.Payments.Where(x => x.ReferenceID == id).ToListAsync(cancellationToken)
        );

        _logger.LogInformation("remove items for purchase({id})", id);
        _appDbContext.PurchaseItems.RemoveRange(
            await _appDbContext.PurchaseItems.Where(x => x.PurchaseID == id).ToListAsync(cancellationToken)
        );

        _logger.LogInformation("remove purchase({id})", id);
        _appDbContext.Purchases.Remove(
            (await _appDbContext.Purchases.FindAsync(id, cancellationToken))!
        );

        _logger.LogInformation("remove purchase({id}) done", id);
        return Result.Success();
    }

    private async Task<Result> returnQuantity
        (Guid purchaseID, Guid productID, float returnedQuantity, CancellationToken cancellationToken = default)
    {
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID);

        if (productInventory is null)
        {
            _logger.LogError("not found product inventory existance)");
            return Result.Failure(GeneralErrors.UnexpectedError);
        }

        productInventory.CurrentQuantity += returnedQuantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _appDbContext.StockTransactions.RemoveRange(
            await _appDbContext.StockTransactions
                .Where(x => x.ReferenceID == purchaseID && x.ProductID == productID)
                .ToListAsync(cancellationToken)
        );

        _appDbContext.Inventory.Update(productInventory);
        _logger.LogInformation("return ended");
        return Result.Success();
    }

    public async Task<Result> UpdatePurchasePaidAmount
        (Guid id, PurchaseUpdatePaidRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check purchase({id}) existance", id);
        if (await _appDbContext.Purchases.FindAsync(id, cancellationToken) is not { } purchase)
        {
            _logger.LogError("not found purchase({id})", id);
            return Result.Failure(PurchaseErrors.NotFound);
        }

        _logger.LogInformation("check purchase({id}) status if already 'Paid'", id);
        if (purchase.Status == PayStatuses.Paid)
        {
            _logger.LogError("purchase({id}) status is already paid", id);
            return Result.Failure(PurchaseErrors.AlreadyPaid);
        }

        var paid = request.PaidAmount + purchase.PaidAmount;
        var status = purchase.Status;
        _logger.LogInformation("check purchase({id}) paid if greater than total amount", id);
        if (paid > purchase.TotalAmount)
        {
            _logger.LogError("paid amount is greater than purchase({id}) total amount", id);
            return Result.Failure(PurchaseErrors.PaidMoreThanTotal);
        }

        var payment = new Payment
        {
            ReferenceID = purchase.Id,
            ReferenceType = ReferenceTypes.Purchase,
            PayMethod = PaymentMethod.Cash,
            Amount = request.PaidAmount
        };
        if (paid == purchase.TotalAmount)
        {
            _logger.LogInformation("update purchase({id}) status to 'Paid'", id);
            status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            _logger.LogInformation("add payment({id}) for purchase({id})", payment.Id, id);
        }
        else if (purchase.PaidAmount < purchase.TotalAmount)
        {
            _logger.LogInformation("update purchase({id}) status to 'NotCompleted'", id);
            status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
            _logger.LogInformation("add payment({id}) for purchase({id})", payment.Id, id);
        }
        else
        {
            _logger.LogInformation("update purchase({id}) status to 'NotPaid'", id);
            status = PayStatuses.NotPaid;
        }

        purchase.PaidAmount = paid;
        purchase.Status = status;
        _appDbContext.Purchases.Update(purchase);
        _logger.LogInformation("execute update purchase({id}) done", id);
        return Result.Success();
    }
}
