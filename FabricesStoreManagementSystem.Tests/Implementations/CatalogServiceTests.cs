namespace FabricesStoreManagementSystem.Tests.Implementations;

public class CatalogServiceTests
{
    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetPayForCatalogFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task PayForCatalog_ShouldFail
        (CatalogFormPurchaseCatalogRequest? temp, PurchaseUpdatePaidRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var id = Guid.Empty;
        if (temp is not null)
        {
            var addTmp = await service.PurchaseCatalog(temp);
            if (addTmp.IsSuccess)
            {
                id = addTmp.Value.Id;
                await db.SaveChangesAsync();
            }
        }
        var result = await service.PayForCatalog(id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetPayForCatalogSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task PayForCatalog_ShouldSuccess
        (CatalogFormPurchaseCatalogRequest temp, PurchaseUpdatePaidRequest request, bool isPaid)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var addTmp = await service.PurchaseCatalog(temp);
        if (addTmp.IsSuccess)
            await db.SaveChangesAsync();
        var result = await service.PayForCatalog(addTmp.Value.Id, request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var payment = await db.Payments.SingleOrDefaultAsync(x => x.ReferenceID == addTmp.Value.Id);
        payment.Should().NotBeNull();
        payment.Amount.Should().Be(request.PaidAmount);
        var catalog = await db.Catalogs.FindAsync(addTmp.Value.Id);
        catalog.Should().NotBeNull();
        catalog.IsPaid.Should().Be(isPaid);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogPurchaseSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task PurchaseCatalog_ShouldSuccess
        (CatalogFormPurchaseCatalogRequest request, bool isPaid)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.PurchaseCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(ProductsRepo.Products()[0].Code);
        result.Value.Status.Should().Be(CatalogStatus.Available);
        result.Value.IsPaid.Should().Be(isPaid);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogPurchaseFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task PurchaseCatalog_ShouldFail
        (CatalogFormPurchaseCatalogRequest request, Error errCode)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.PurchaseCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(errCode);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogCreateSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task CreateCatalog_ShouldSuccess
        (CatalogRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(ProductsRepo.Products()[0].Code);
        result.Value.Status.Should().Be(CatalogStatus.Available);

        var product = request.Items[0];
        var inventory = await db.Inventory.SingleOrDefaultAsync(x => x.ProductID == product.ProductID);
        inventory.Should().NotBeNull();
        inventory.CurrentQuantity.Should().Be(ProductInventoriesRepo.Inventories().Single(x => x.ProductID == product.ProductID).CurrentQuantity - product.Quantity);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogCreateFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task CreateCatalog_ShouldFail
        (CatalogRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogCreateBySupplierSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task CreateCatalogBySupplier_ShouldSuccess
        (CatalogFromSupplierRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(ProductsRepo.Products()[0].Code);
        result.Value.Status.Should().Be(CatalogStatus.Available);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogCreateBySupplierFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task CreateCatalogBySupplier_ShouldFail
        (CatalogFromSupplierRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogRemoveSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task RemoveCatalog_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveCatalog(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var catalog = await db.Catalogs.FindAsync(id);
        catalog.Should().BeNull();

        var catalogProducts = await db.CatalogsProducts.Where(x => x.CatalogID == id).CountAsync();
        catalogProducts.Should().Be(0);
        var catalogAssings = await db.CatalogsAssigns.Where(x => x.CatalogID == id).CountAsync();
        catalogAssings.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogRemoveFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task RemoveCatalog_ShouldFail
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.RemoveCatalog(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task GetCatalog_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCatalog(id);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task GetCatalog_ShouldFail
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCatalog(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogsSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task GetCatalogs_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCatalogs(paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogDestructeSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task DestructionCatalog_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.DestructionCatalog(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogDestructeFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task DestructionCatalog_ShouldFail
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.DestructionCatalog(id);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogReturnSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task ReturnCatalog_ShouldSuccess
        (Guid assignID)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.ReturnCatalog(assignID);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ReturnedAt.Should().NotBeNull();
        var catalog = await db.Catalogs.FindAsync(result.Value.CatalogID);
        catalog.Should().NotBeNull();
        catalog.Status.Should().Be(CatalogStatus.Available);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogReturnFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task ReturnCatalog_ShouldFail
        (Guid assignID, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.ReturnCatalog(assignID);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogAssingSuccessTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task AssignCatalog_ShouldSuccess
        (AssignCatalogRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.AssignCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var catalog = await db.Catalogs.FindAsync(result.Value.CatalogID);
        catalog.Should().NotBeNull();
        catalog.Status.Should().Be(CatalogStatus.Assigned);
    }

    [Theory]
    [MemberData(nameof(CatalogServiceTestsHelpers.GetCatalogAssingFailTestsData), MemberType = typeof(CatalogServiceTestsHelpers))]
    public async Task AssignCatalog_ShouldFail
        (AssignCatalogRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CatalogService>.Instance;
        var service = new CatalogService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.AssignCatalog(request);
        if (result.IsSuccess)
            await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}
