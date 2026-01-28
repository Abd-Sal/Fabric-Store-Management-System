namespace FabricesStoreManagementSystem.Implementations;

public class SaleService(AppDbContext appDbContext, ILogger<SaleService> logger) : ISaleService
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<SaleService> _logger = logger;

    public async Task<Result<SaleResponse>> CreateSale
        (SaleRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for sale items uniqueness");
        if (request.SaleItems.Count != request.SaleItems.DistinctBy(x => x.ProductID).Count())
            return Result.Failure<SaleResponse>(ProductErrors.DuplicatedInInvoice);

        _logger.LogInformation("check for customer existance");
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
        _logger.LogInformation("sale with id({id}) added without items and still processing", sale.Id);
        var processSaleItems = await CreateSaleItems(sale.Id, request.SaleItems, cancellationToken);
        if (processSaleItems.IsFailure)
            return Result.Failure<SaleResponse>(processSaleItems.Error);
        var resultSaleItems = processSaleItems.Value;

        _logger.LogInformation("sale items added and still processing");

        sale.ProductsCount = resultSaleItems.Count;
        sale.TotalAmount = resultSaleItems.Sum(x => x.Total);

        var netAmount = sale.TotalAmount - request.Discount;
        sale.NetAmount = netAmount;

        _logger.LogInformation("updating sale.ProductCount, sale.TotalAmount, sale.NetAmount for sale with id({id})", sale.Id);

        _logger.LogInformation("check if net amount less than 0");
        if (netAmount < 0)
        {
            sale.NetAmount = 0;
            _logger.LogInformation("net amount less than so it will equal 0");
        }


        sale.PaidAmount = request.PaidAmount;
        _logger.LogInformation("updating sale.PaidAmount");

        var payment = new Payment
        {
            ReferenceID = sale.Id,
            ReferenceType = ReferenceTypes.Sale,
            PayMethod = PaymentMethod.Cash,
            Amount = sale.PaidAmount
        };

        if (sale.PaidAmount > sale.NetAmount)
        {
            _logger.LogError("paid amount is more than net amount");
            return Result.Failure<SaleResponse>(SaleErrors.PaidMoreThanNetTotal);
        }
        else if (sale.PaidAmount == sale.NetAmount)
        {
            sale.Status = PayStatuses.Paid;
            _logger.LogInformation("net amount and paid amount is equal so update sale status to 'Paid'");
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (sale.PaidAmount < sale.NetAmount)
        {
            _logger.LogInformation("paid amount less than net amount so the sale status is 'NotCompleted'");
            sale.Status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
        {
            _logger.LogInformation("paid amount equal 0 so the sale status us 'NotPaid'");
            sale.Status = PayStatuses.NotPaid;
        }

        _appDbContext.Sales.Update(sale);
        _logger.LogInformation("Sale with id({id}) Done", sale.Id);
        return Result.Success(sale.ToSaleResponseWithNoItems());
    }

    private async Task<Result<List<SaleItem>>> CreateSaleItems
        (Guid saleID, List<SaleItemRequest> items, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("start create sale items for with sale id({id})", saleID);
        var processItems = items
            .Select(x => new SaleItem
            {
                ProductID = x.ProductID,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                SaleID = saleID,
            })
            .Select(x => CreateSaleItem(x, cancellationToken));
        var results = Task.WhenAll(processItems).Result;
        if (results is null || results.Length == 0)
            return Result.Failure<List<SaleItem>>(SaleErrors.NoSuccessfulSaleItems);
        _logger.LogInformation("end process adding sale items");
        foreach (var r in results)
            if(r.IsFailure)
                return Result.Failure<List<SaleItem>>(r.Error);
        _logger.LogInformation("no errors in adding sale items");
        var res = results.Select(x => x.Value).ToList();
        _logger.LogInformation("sale items done for sale id({id})", saleID);
        return Result.Success(res);
    }
    
    private async Task<Result<SaleItem>> CreateSaleItem
        (SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check product existance");
        if (!(await _appDbContext.Products.AsNoTracking().AnyAsync(x => x.Id == saleItem.ProductID, cancellationToken)))
            return Result.Failure<SaleItem>(ProductErrors.NotFound);

        _logger.LogInformation("check inventory existance");
        if (await _appDbContext.Inventory.SingleOrDefaultAsync(x => x.ProductID == saleItem.ProductID, cancellationToken) is not { } productInventory)
            return Result.Failure<SaleItem>(ProductErrors.NoQuantity);

        _logger.LogInformation("check current quantity not equal to 0");
        if (productInventory.CurrentQuantity == 0)
            return Result.Failure<SaleItem>(ProductErrors.NoQuantity);

        _logger.LogInformation("check current quantity if less than request quantity");
        if (saleItem.Quantity > productInventory.CurrentQuantity)
            return Result.Failure<SaleItem>(ProductErrors.NoEnoughQuantity);

        var stockTransaction = new StockTransaction
        {
            Note = "تم تسجيل عملية بيع من قبل المدير و تم خصم الكمية من المخزن بنجاح",
            ProductID = saleItem.ProductID,
            QuantityChange = -1f * saleItem.Quantity,
            ReferenceID = saleItem.SaleID,
            ReferenceType = ReferenceTypes.Sale,
            TransactionType = StockTransactionType.Sale
        };

        await _appDbContext.SaleItems.AddAsync(saleItem, cancellationToken);
        await _appDbContext.StockTransactions.AddAsync(stockTransaction, cancellationToken);
        productInventory.CurrentQuantity = productInventory.CurrentQuantity - saleItem.Quantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;
        _appDbContext.Inventory.Update(productInventory);
        _logger.LogInformation("add sale item with id({itemID}), add stock transaction with id({transID}), update product inventory details", saleItem.Id, stockTransaction.Id);
        return Result.Success(saleItem);
    }

    public async Task<Result> UpdateSalePaidAmount
        (Guid id, SaleUpdatePaidRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("start updating sale paid amount with id({id})", id);
        if (await _appDbContext.Sales.FindAsync(id, cancellationToken) is not { } sale)
            return Result.Failure(SaleErrors.NotFound);

        _logger.LogInformation("check if the sale is paid or not");
        if (sale.Status == PayStatuses.Paid)
            return Result.Failure(SaleErrors.AlreadyPaid);

        _logger.LogInformation("increase paid amount");
        var paid = request.PaidAmount + sale.PaidAmount;
        var status = sale.Status;
        _logger.LogInformation("check if paid amount greater than net amount");
        if (paid > sale.NetAmount)
            return Result.Failure(SaleErrors.PaidMoreThanNetTotal);

        var payment = new Payment
        {
            ReferenceID = sale.Id,
            ReferenceType = ReferenceTypes.Sale,
            PayMethod = PaymentMethod.Cash,
            Amount = sale.PaidAmount
        };
        if (paid == sale.NetAmount)
        {
            _logger.LogInformation("update status to 'Paid'");
            status = PayStatuses.Paid;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else if (sale.PaidAmount < sale.NetAmount)
        {
            _logger.LogInformation("update status to 'NotCompleted'");
            status = PayStatuses.NotCompleted;
            await _appDbContext.Payments.AddAsync(payment, cancellationToken);
        }
        else
        {
            _logger.LogInformation("update status to 'NotPiad'");
            status = PayStatuses.NotPaid;
        }

        _logger.LogInformation("update status to 'NotPiad'");
        await _appDbContext.Sales.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.PaidAmount, paid)
                    .SetProperty(x => x.Status, status),
                cancellationToken
            );
        _logger.LogInformation("updating status done id({id})", id);
        return Result.Success();
    }

    public async Task<Result> RemoveSale
        (Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("check for sale with id({id}) if existance", id);
        var sale = await _appDbContext.Sales
            .Include(x => x.SaleItems)
            .SingleOrDefaultAsync(x => x.Id == id);

        if(sale is null)
            return Result.Failure(SaleErrors.NotFound);

        _logger.LogInformation("start returning the products and adjust quantities");
        var returnProductQuantities = sale.SaleItems
            .Select(x => returnQuantity(id, x.ProductID, x.Quantity, cancellationToken));

        var endedReturnQuantities = await Task.WhenAll(returnProductQuantities);
        _logger.LogInformation("end return products to stock");
        _logger.LogInformation("check if there is error in returning process");
        foreach (var ended in endedReturnQuantities)
            return Result.Failure(ended.Error);

        await _appDbContext.Payments.Where(x => x.ReferenceID == id)
            .ExecuteDeleteAsync(cancellationToken);
        _logger.LogInformation("remove payment");

        await _appDbContext.SaleItems.Where(x => x.SaleID == id)
            .ExecuteDeleteAsync(cancellationToken);
        _logger.LogInformation("remove sale items");

        await _appDbContext.Sales.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);
        _logger.LogInformation("remove sale id({id})", id);

        return Result.Success();
    }

    private async Task<Result> returnQuantity
        (Guid saleID, Guid productID, float returnedQuantity, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("start return product with id({id}) for sale id({saleID})", productID, saleID);
        _logger.LogInformation("check product inventory existance");
        var productInventory = await _appDbContext.Inventory
            .SingleOrDefaultAsync(x => x.ProductID == productID);

        _logger.LogInformation("check inventory existance of product({id})",productID);
        if (productInventory is null)
            return Result.Failure(GeneralErrors.UnexpectedError);

        _logger.LogInformation("add returned quantity");
        productInventory.CurrentQuantity += returnedQuantity;
        productInventory.LastUpdateAt = DateTime.UtcNow;

        _logger.LogInformation("remove stock transaction");
        _appDbContext.StockTransactions.RemoveRange(
            await _appDbContext.StockTransactions
                .Where(x => x.ReferenceID == saleID && x.ProductID == productID)
                .ToListAsync(cancellationToken)
        );

        _appDbContext.Inventory.Update(productInventory);
        _logger.LogInformation("update product inventory done");
        return Result.Success();
    }

    public async Task<Result<PaginatedList<SaleResponse>>> GetSales
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, SearchRequest? searchRequest, CancellationToken cancellationToken = default)
    {
        var query = _appDbContext.Sales.AsNoTracking();

        if(dateRangeRequest is not null)
            query = query
                .Where(x => DateOnly.FromDateTime(x.CreatedAt) >= dateRangeRequest.From &&
                            DateOnly.FromDateTime(x.CreatedAt) <= dateRangeRequest.To);

        if (searchRequest is not null)
            query = query
                .Where(x => SaleSearchs.SaleResponseSearch(searchRequest).ToString().ToLower().Contains(searchRequest.Search));

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
}
