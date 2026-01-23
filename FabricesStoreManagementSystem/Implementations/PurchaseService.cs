namespace FabricesStoreManagementSystem.Implementations;

public class PurchaseService(AppDbContext appDbContext, IProductService productService) : IPurchaseService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly IProductService _productService = productService;

    public async Task<Result<PurchaseResponse>> CreatePurchase
        (PurchaseRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PurchaseItems.Count() != request.PurchaseItems.DistinctBy(x => x.ProductID).Count())
            return Result.Failure<PurchaseResponse>(ProductErrors.DuplicatedInInvoice);

        if (!(await _appDbContext.Suppliers.AsNoTracking()
            .AnyAsync(x => x.Id == request.SupplierID && x.IsActive, cancellationToken)))
            return Result.Failure<PurchaseResponse>(SupplierErrors.NotFound);

        var purchase = new Purchase
        {
            InvoiceNumber = HelperTools.GenerateInvoiceNumber(),
            SupplierID = request.SupplierID,
            ProductsCount = request.PurchaseItems.Count,
            Status = PayStatuses.NotPaid,
        };

        await _appDbContext.Purchases.AddAsync(purchase, cancellationToken);
        var processPurchaseItems = await CreatePurchaseItems(purchase.Id, request.PurchaseItems, cancellationToken);
        if (processPurchaseItems.IsFailure)
            return Result.Failure<PurchaseResponse>(processPurchaseItems.Error);
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

        if (purchase.PaidAmount > purchase.TotalAmount)
            return Result.Failure<PurchaseResponse>(PurchaseErrors.PaidMoreThanTotal);
        else if (purchase.PaidAmount == purchase.TotalAmount)
        {
            purchase.Status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (purchase.PaidAmount < purchase.TotalAmount)
        {
            purchase.Status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
            purchase.Status = PayStatuses.NotPaid;

        _appDbContext.Purchases.Update(purchase);
        return Result.Success(purchase.ToPurchaseResponseWithoutItems());
    }

    private async Task<Result<List<PurchaseItem>>> CreatePurchaseItems
        (Guid id, List<PurchaseItemRequest> items, CancellationToken cancellationToken)
    {
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
        if (results is null || results.Length == 0)
            return Result.Failure<List<PurchaseItem>>(PurchaseErrors.NoSuccessfulPurchsaeItems);

        foreach (var r in results)
            if (r.IsFailure)
                return Result.Failure<List<PurchaseItem>>(r.Error);
        var res = results.Select(x => x.Value).ToList();
        return Result.Success(res);

    }

    private async Task<Result<PurchaseItem>> CreatePurchaseItem
        (PurchaseItem purchaseItem, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == purchaseItem.ProductID, cancellationToken)))
            return Result.Failure<PurchaseItem>(ProductErrors.NotFound);

        if (await _appDbContext.Inventory.SingleOrDefaultAsync(x => x.ProductID == purchaseItem.ProductID, cancellationToken) is not { } productInventory)
            return Result.Failure<PurchaseItem>(ProductErrors.NoQuantity);

        var stockTransaction = new StockTransaction
        {
            Note = "Do purchase process and inecrease quantity by admin",
            ProductID = purchaseItem.ProductID,
            QuantityChange = purchaseItem.Quantity,
            ReferenceID = purchaseItem.PurchaseID,
            ReferenceType = ReferenceTypes.Purchase,
            TransactionType = StockTransactionType.Purchase
        };

        await _appDbContext.PurchaseItems.AddAsync(purchaseItem, cancellationToken);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        productInventory.CurrentQuantity += purchaseItem.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Inventory.Update(productInventory);

        return Result.Success(purchaseItem);
    }

    public async Task<Result<PaginatedList<PurchaseResponse>>> GetPurchases
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Purchases.AsNoTracking();

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
        var purchase = await _appDbContext.Purchases
            .Include(x => x.PurchaseItems)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (purchase is null)
            return Result.Failure(PurchaseErrors.NotFound);

        var getSaleProcesses = purchase.PurchaseItems
            .Select(x => _productService
                            .GetSalesByProduct(x.ProductID,
                                                new PaginationRequest(1, 1),
                                                new SortRequest(null, null),
                                                new DateRangeRequest(DateOnly.FromDateTime(purchase.CreatedAt), DateOnly.FromDateTime(DateTime.UtcNow)),
                                                cancellationToken));

        var salesResultProcess = await Task.WhenAll(getSaleProcesses);
        foreach (var item in salesResultProcess)
            return Result.Failure(item.Error);

        var check = salesResultProcess.Select(x => x.Value.TotalItems).Max();
        if (check > 0)
            return Result.Failure(PurchaseErrors.UnableToReturnPurchase);

        var returnProductQuantities = purchase.PurchaseItems
            .Select(x => returnQuantity(id, x.ProductID, x.Quantity, cancellationToken));

        var endedReturnQuantities = await Task.WhenAll(returnProductQuantities);
        foreach (var ended in endedReturnQuantities)
            return Result.Failure(ended.Error);

        await _appDbContext.Payments.Where(x => x.ReferenceID == id)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.PurchaseItems.Where(x => x.PurchaseID == id)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.Purchases.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> returnQuantity
        (Guid purchaseID, Guid productID, float returnedQuantity, CancellationToken cancellationToken = default)
    {
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID);

        if (productInventory is null)
            return Result.Failure(GeneralErrors.UnexpectedError);

        productInventory.CurrentQuantity += returnedQuantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _appDbContext.StockTransactions.RemoveRange(
            await _appDbContext.StockTransactions
                .Where(x => x.ReferenceID == purchaseID && x.ProductID == productID)
                .ToListAsync(cancellationToken)
        );

        _appDbContext.Inventory.Update(productInventory);
        return Result.Success();
    }

    public async Task<Result> UpdatePurchasePaidAmount
        (Guid id, PurchaseUpdatePaidRequest request, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Purchases.FindAsync(id, cancellationToken) is not { } purchase)
            return Result.Failure(PurchaseErrors.NotFound);

        if (purchase.Status == PayStatuses.Paid)
            return Result.Failure(PurchaseErrors.AlreadyPaid);

        var paid = request.PaidAmount + purchase.PaidAmount;
        var status = purchase.Status;
        if (paid > purchase.TotalAmount)
            return Result.Failure(PurchaseErrors.PaidMoreThanTotal);

        var payment = new Payment
        {
            ReferenceID = purchase.Id,
            ReferenceType = ReferenceTypes.Purchase,
            PayMethod = PaymentMethod.Cash,
            Amount = purchase.PaidAmount
        };
        if (paid == purchase.TotalAmount)
        {
            status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (purchase.PaidAmount < purchase.TotalAmount)
        {
            status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
            status = PayStatuses.NotPaid;

        await _appDbContext.Purchases.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.PaidAmount, paid)
                    .SetProperty(x => x.Status, status),
                cancellationToken
            );
        return Result.Success();
    }
}
