namespace FabricesStoreManagementSystem.Tests.Implementations;

public class SaleServiceTests
{
    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleCreateSuccessTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task CreateSale_ShouldSuccess
        (SaleRequest request, PayStatuses status)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateSale(request);
        if(result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var Sale = await db.Sales.FindAsync(result.Value.Id);
        Sale.Should().NotBeNull();
        Sale.ProductsCount.Should().Be(request.SaleItems.Count);

        var stockTransactions = await db.StockTransactions
            .Where(x => x.ReferenceID == result.Value.Id)
            .ToListAsync();
        stockTransactions.Should().NotBeNull();
        stockTransactions.Count.Should().Be(request.SaleItems.Count);

        var payments = await db.Payments.Where(x => x.ReferenceID == result.Value.Id)
            .ToListAsync();
        payments.Should().NotBeNull();
        if(status != PayStatuses.NotPaid)
            payments.Count.Should().Be(1);

        var product = request.SaleItems[0];
        var inventory = await db.Inventory.SingleOrDefaultAsync(x => x.ProductID == product.ProductID);
        inventory.Should().NotBeNull();
        var oldInv = ProductInventoriesRepo.Inventories().SingleOrDefault(x => x.ProductID == product.ProductID)!;
        inventory.CurrentQuantity.Should().Be(oldInv.CurrentQuantity - product.Quantity);
        result.Value.Status.Should().Be(status);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleCreateFailTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task CreateSale_ShouldFail
        (SaleRequest request, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories().Slice(0, 11));
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateSale(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var Sale = await db.SaleItems.SingleOrDefaultAsync(x => x.SaleID == request.SaleItems.First().ProductID);
        Sale.Should().BeNull();

        var stockTransactions = await db.StockTransactions
            .Where(x => x.ProductID == request.SaleItems.First().ProductID)
            .ToListAsync();
        stockTransactions.Count.Should().Be(0);

        var payments = await db.Payments
            .Where(x => x.ReferenceID == request.SaleItems.First().ProductID)
            .ToListAsync();
        payments.Count.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSalesSuccessTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task GetSales_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest? dateRangeRequest, SearchRequest? searchRequest)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSales(paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleSuccessTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task GetSale_ShouldSuccess
        (Guid id)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSale(id);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleFailTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task GetSale_ShouldFail
        (Guid id, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSale(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [InlineData("20260103")]
    [InlineData("20260102")]
    [InlineData("20260101")]
    public async Task GetSaleByInvoiceNumber_ShouldSuccess
        (string invoiceNumber)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSaleByInvoiceNumber(invoiceNumber);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("202601010", "Sale.NotFound")]
    [InlineData("202601011", "Sale.NotFound")]
    [InlineData("202601012", "Sale.NotFound")]
    public async Task GetSaleByInvoiceNumber_ShouldFail
        (string invoiceNumber, string errCode)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSaleByInvoiceNumber(invoiceNumber);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(errCode);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleRemoveSuccessTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task RemoveSale_ShouldSuccess
        (Guid id)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveSale(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSaleRemoveFailTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task RemoveSale_ShouldFail
        (Guid id, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveSale(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSalePayAmountSuccessTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task UpdateSalePaidAmount_ShouldSuccess
        (Guid id, SaleUpdatePaidRequest request, PayStatuses status)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateSalePaidAmount(id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var sale = await db.Sales.FindAsync(id);
        sale.Should().NotBeNull();
        sale.Status.Should().Be(status);

        var payment = await db.Payments.SingleOrDefaultAsync(x => x.ReferenceID == id);
        payment.Should().NotBeNull();
        payment.Amount.Should().Be(request.PaidAmount);
    }

    [Theory]
    [MemberData(nameof(SaleServiceTestsHelpers.GetSalePayAmountFailTestsData), MemberType = typeof(SaleServiceTestsHelpers))]
    public async Task UpdateSalePaidAmount_ShouldFail
        (Guid id, SaleUpdatePaidRequest request, Error error)
    {
        //Arrrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SaleService>.Instance;
        var productLogger = NullLogger<ProductService>.Instance;
        var service = new SaleService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Sales.AddRangeAsync(SalesRepo.Sales());
        await db.SaleItems.AddRangeAsync(SaleItemsRepo.SaleItems());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateSalePaidAmount(id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}

