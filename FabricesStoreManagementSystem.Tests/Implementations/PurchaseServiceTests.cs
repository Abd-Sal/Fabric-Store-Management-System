namespace FabricesStoreManagementSystem.Tests.Implementations;

public class PurchaseServiceTests
{
    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchasePayFailTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task UpdatePurchasePaidAmount_ShouldFail
        (Guid id, PurchaseUpdatePaidRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdatePurchasePaidAmount(id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchasePaySuccessTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task UpdatePurchasePaidAmount_ShouldSuccess
        (Guid id, PurchaseUpdatePaidRequest request, PayStatuses status)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdatePurchasePaidAmount(id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var payment = await db.Payments.SingleOrDefaultAsync(x => x.ReferenceID == id);
        payment.Should().NotBeNull();
        payment.Amount.Should().Be(request.PaidAmount);
        var purchase = await db.Purchases.FindAsync(id);
        purchase.Should().NotBeNull();
        purchase.Status.Should().Be(status);
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseCreateSuccessTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task CreatePurchase_ShouldSuccess
        (PurchaseRequest request, PayStatuses status)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreatePurchase(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var purchase = await db.Purchases.FindAsync(result.Value.Id);
        purchase.Should().NotBeNull();
        purchase.ProductsCount.Should().Be(request.PurchaseItems.Count);

        var stockTransactions = await db.StockTransactions
            .Where(x => x.ReferenceID == result.Value.Id)
            .ToListAsync();
        stockTransactions.Should().NotBeNull();
        stockTransactions.Count.Should().Be(request.PurchaseItems.Count);
        result.Value.Status.Should().Be(status);
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseCreateFailTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task CreatePurchase_ShouldFail
        (PurchaseRequest request, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreatePurchase(request);
        if(result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var purchase = await db.PurchaseItems.SingleOrDefaultAsync(x => x.PurchaseID == request.PurchaseItems.First().ProductID);
        purchase.Should().BeNull();

        var stockTransactions = await db.StockTransactions
            .Where(x => x.ProductID == request.PurchaseItems.First().ProductID)
            .ToListAsync();
        stockTransactions.Count.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchasesSuccessTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task GetPurchases_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchases(paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseSuccessTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task GetPurchase_ShouldSuccess
        (Guid id)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchase(id);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseFailTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task GetPurchase_ShouldFail
        (Guid id, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchase(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [InlineData("20260103")]
    [InlineData("20260102")]
    [InlineData("20260101")]
    public async Task GetPurchaseByInvoiceNumber_ShouldSuccess
        (string invoiceNumber)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchaseByInvoiceNumber(invoiceNumber);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
 
    [Theory]
    [InlineData("20260104", "Purchase.NotFound")]
    [InlineData("20260105", "Purchase.NotFound")]
    [InlineData("20260106", "Purchase.NotFound")]
    public async Task GetPurchaseByInvoiceNumber_ShouldFail
        (string invoiceNumber, string errCode)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchaseByInvoiceNumber(invoiceNumber);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(errCode);
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseRemoveSuccessTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task RemovePurchase_ShouldSuccess
        (Guid id)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemovePurchase(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(PurchaseServiceTestsHelpers.GetPurchaseRemoveFailTestsData), MemberType = typeof(PurchaseServiceTestsHelpers))]
    public async Task RemovePurchase_ShouldFail
        (Guid id, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<PurchaseService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var productService = new ProductService(db, productLogger);
        var service = new PurchaseService(db, productService, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Purchases.AddRangeAsync(PurchasesRepo.Purchases());
        await db.PurchaseItems.AddRangeAsync(PurchaseItemsRepo.PurchaseItems());
        var sale = new Sale
        {
            CustomerID = CustomersRepo.Customers()[0].Id,
            ProductsCount = 1,
            Discount = 1,
            NetAmount = 10,
            TotalAmount = 11,
            PaidAmount = 10,
            Status = PayStatuses.Paid
        };
        var saleItems = new SaleItem
        {
            ProductID = ProductsRepo.Products()[5].Id,
            Quantity = 2,
            UnitPrice = 50,
            SaleID = sale.Id
        };
        await db.Sales.AddAsync(sale);
        await db.SaleItems.AddAsync(saleItems);
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemovePurchase(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
