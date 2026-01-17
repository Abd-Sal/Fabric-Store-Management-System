namespace FabricesStoreManagementSystem.Implementations;

public class PurchaseService(AppDbContext appDbContext) : IPurchaseService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<PurchaseResponse>> CreatePurchase
        (PurchaseRequest request, CancellationToken cancellationToken = default)
    {
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

    public async Task<Result<List<PurchaseResponse>>> GetPurchases
        (CancellationToken cancellationToken = default)
    {
        var purchases = await _appDbContext.Purchases.AsNoTracking()
            .Select(x => x.ToPurchaseResponseWithoutItems())
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result.Success(purchases);
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

    public async Task<Result<List<PurchaseResponse>>> GetPurchaseByRangeDate
        (DateRangeRequest dateRange, CancellationToken cancellationToken = default)
    {
        var purchases = await _appDbContext.Purchases.AsNoTracking()
            .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRange.From &&
                        DateOnly.Parse(x.CreatedAt.ToString()) <= dateRange.To)
            .Select(x => x.ToPurchaseResponseWithoutItems())
            .ToListAsync(cancellationToken);

        return Result.Success(purchases);
    }
}
