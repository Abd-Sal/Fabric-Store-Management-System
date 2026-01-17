namespace FabricesStoreManagementSystem.Implementations;

public class SaleService(AppDbContext appDbContext) : ISaleService
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<SaleResponse>> CreateSale
        (SaleRequest request, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Customers.AsNoTracking()
            .AnyAsync(x => x.Id == request.CustomerID && x.IsActive, cancellationToken)))
            return Result.Failure<SaleResponse>(CustomerErrors.NotFound);

        var sale = new Sale
        {
            InvoiceNumber = HelperTools.GenerateInvoiceNumber(),
            CustomerID = request.CustomerID,
            ProductsCount = request.SaleItems.Count,
            Status = PayStatuses.NotPaid,
            Discount = request.Discount,
        };

        await _appDbContext.Sales.AddAsync(sale, cancellationToken);
        var processSaleItems = await CreateSaleItems(sale.Id, request.SaleItems, cancellationToken);
        if (processSaleItems.IsFailure)
            return Result.Failure<SaleResponse>(processSaleItems.Error);
        var resultSaleItems = processSaleItems.Value;

        sale.ProductsCount = resultSaleItems.Count;
        sale.NetAmount = resultSaleItems.Sum(x => x.Total);

        var totalAmount = resultSaleItems.Sum(x => x.Total) - request.Discount;
        sale.TotalAmount = totalAmount;

        if (totalAmount > 0)
            sale.TotalAmount = 0;

        sale.PaidAmount = request.PaidAmount;

        var payment = new Payment
        {
            ReferenceID = sale.Id,
            ReferenceType = ReferenceTypes.Sale,
            PayMethod = PaymentMethod.Cash,
            Amount = sale.PaidAmount
        };

        if (sale.PaidAmount > sale.TotalAmount)
            return Result.Failure<SaleResponse>(SaleErrors.PaidMoreThanTotal);
        else if (sale.PaidAmount == sale.TotalAmount)
        {
            sale.Status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (sale.PaidAmount < sale.TotalAmount)
        {
            sale.Status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
            sale.Status = PayStatuses.NotPaid;


        _appDbContext.Sales.Update(sale);

        return Result.Success(sale.ToSaleResponseWithNoItems());
    }

    private async Task<Result<List<SaleItem>>> CreateSaleItems
        (Guid saleID, List<SaleItemRequest> items, CancellationToken cancellationToken = default)
    {
        var processItems = items
            .Select(x => new SaleItem
            {
                ProductID = x.ProductID,
                Quantity = x.Qunatity,
                UnitPrice = x.UnitPrice,
                SaleID = saleID,
            })
            .Select(x => CreateSaleItem(x, cancellationToken));
        var results = Task.WhenAll(processItems).Result;
        if (results is null || results.Length == 0)
            return Result.Failure<List<SaleItem>>(SaleErrors.NoSuccessfulSaleItems);
        foreach (var r in results)
            if(r.IsFailure)
                return Result.Failure<List<SaleItem>>(r.Error);
        var res = results.Select(x => x.Value).ToList();
        return Result.Success(res);
    }
    
    private async Task<Result<SaleItem>> CreateSaleItem
        (SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == saleItem.ProductID, cancellationToken)))
            return Result.Failure<SaleItem>(ProductErrors.NotFound);

        if (await _appDbContext.Inventory.SingleOrDefaultAsync(x => x.ProductID == saleItem.ProductID, cancellationToken) is not { } productInventory)
            return Result.Failure<SaleItem>(ProductErrors.NoQuantity);

        if(productInventory.CurrentQuantity == 0)
            return Result.Failure<SaleItem>(ProductErrors.NoQuantity);

        if (saleItem.Quantity > productInventory.CurrentQuantity)
            return Result.Failure<SaleItem>(ProductErrors.NoEnoughQuantity);

        var stockTransaction = new StockTransaction
        {
            Note = "Do sale process and decrease quantity by admin",
            ProductID = saleItem.ProductID,
            QuantityChange = -1 * saleItem.Quantity,
            ReferenceID = saleItem.SaleID,
            ReferenceType = ReferenceTypes.Sale,
            TransactionType = StockTransactionType.Sale
        };

        await _appDbContext.SaleItems.AddAsync(saleItem, cancellationToken);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        productInventory.CurrentQuantity = productInventory.CurrentQuantity - saleItem.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Inventory.Update(productInventory);
        
        return Result.Success(saleItem);
    }

    public async Task<Result> UpdateSalePaidAmount
        (Guid id, SaleUpdatePaidRequest request, CancellationToken cancellationToken = default)
    {
        if (await _appDbContext.Sales.FindAsync(id, cancellationToken) is not { } sale)
            return Result.Failure(SaleErrors.NotFound);

        if (sale.Status == PayStatuses.Paid)
            return Result.Failure(SaleErrors.AlreadyPaid);

        var paid = request.PaidAmount + sale.PaidAmount;
        var status = sale.Status;
        if (paid > sale.TotalAmount)
            return Result.Failure(SaleErrors.PaidMoreThanTotal);

        var payment = new Payment
        {
            ReferenceID = sale.Id,
            ReferenceType = ReferenceTypes.Sale,
            PayMethod = PaymentMethod.Cash,
            Amount = sale.PaidAmount
        };
        if (paid == sale.TotalAmount)
        {
            status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (sale.PaidAmount < sale.TotalAmount)
        {
            status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
            status = PayStatuses.NotPaid;

        await _appDbContext.Sales.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.PaidAmount, paid)
                    .SetProperty(x => x.Status, status),
                cancellationToken
            );
        return Result.Success();
    }

    public async Task<Result> RemoveSale
        (Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _appDbContext.Sales
            .Include(x => x.SaleItems)
            .SingleOrDefaultAsync(x => x.Id == id);

        if(sale is null)
            return Result.Failure(SaleErrors.NotFound);

        var returnProductQuantities = sale.SaleItems
            .Select(x => returnQuantity(id, x.ProductID, x.Quantity, cancellationToken));

        var endedReturnQuantities = await Task.WhenAll(returnProductQuantities);
        foreach (var ended in endedReturnQuantities)
            return Result.Failure(ended.Error);

        await _appDbContext.Payments.Where(x => x.ReferenceID == id)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.SaleItems.Where(x => x.SaleID == id)
            .ExecuteDeleteAsync(cancellationToken);

        await _appDbContext.Sales.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> returnQuantity
        (Guid saleID, Guid productID, float returnedQuantity, CancellationToken cancellationToken = default)
    {
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID);

        if (productInventory is null)
            return Result.Failure(GeneralErrors.UnexpectedError);

        productInventory.CurrentQuantity += returnedQuantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _appDbContext.StockTransactions.RemoveRange(
            await _appDbContext.StockTransactions
                .Where(x => x.ReferenceID == saleID && x.ProductID == productID)
                .ToListAsync(cancellationToken)
        );

        _appDbContext.Inventory.Update(productInventory);

        return Result.Success();
    }

    public async Task<Result<List<SaleResponse>>> GetSales
        (CancellationToken cancellationToken = default)
    {
        var sales = _appDbContext.Sales.AsNoTracking()
            .Select(x => x.ToSaleResponseWithNoItems())
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result.Success(await sales);
    }

    public async Task<Result<SaleResponse>> GetSale
        (Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _appDbContext.Sales.AsNoTracking()
            .Include(x => x.SaleItems)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (sale is null)
            return Result.Failure<SaleResponse>(SaleErrors.NotFound);

        return Result.Success(sale.ToSaleResponse());
    }

    public async Task<Result<SaleResponse>> GetSaleByInvoiceNumber
        (string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var sale = await _appDbContext.Sales.AsNoTracking()
            .Include(x => x.SaleItems)
            .SingleOrDefaultAsync(x => x.InvoiceNumber == invoiceNumber, cancellationToken);

        if (sale is null)
            return Result.Failure<SaleResponse>(SaleErrors.NotFound);

        return Result.Success(sale.ToSaleResponse());
    }

    public async Task<Result<List<SaleResponse>>> GetSaleByRangeDate
        (DateRangeRequest dateRange, CancellationToken cancellationToken = default)
    {
        var sales = await _appDbContext.Sales.AsNoTracking()
            .Where(x => DateOnly.Parse(x.CreatedAt.ToString()) >= dateRange.From &&
                        DateOnly.Parse(x.CreatedAt.ToString()) <= dateRange.To)
            .Select(x => x.ToSaleResponseWithNoItems())
            .ToListAsync(cancellationToken);

        return Result.Success(sales);
    }
}
