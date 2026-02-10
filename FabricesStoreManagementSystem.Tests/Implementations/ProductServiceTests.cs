namespace FabricesStoreManagementSystem.Tests.Implementations;

public class ProductServiceTests
{
    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetCreateProductsSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task CreateProduct_ShouldSuccess
        (ProductRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        //Act
        var result = await service.CreateProduct(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(request.Code);
        var products = await db.Products.CountAsync();
        products.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetCreateProductsFailTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task CreateProduct_ShouldFail_DuplicationCodeAndColor
        (ProductRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateProduct(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var products = await db.Products.CountAsync();
        products.Should().Be(ProductsRepo.Products().Count);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsShouldSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProduct_ShouldSuccess
        (Guid id)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProduct(id);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsShouldFailTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProduct_ShouldFail_NotFound
        (Guid id, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProduct(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsTransactionsSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProductStockTransactions_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.StockTransactions.AddRangeAsync(ProductStockTransactionsRepo.StockTransactions());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProductStockTransactions(id, paginationRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalItems.Should().Be(ProductStockTransactionsRepo.StockTransactions().Count(x => x.ProductID == id));
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsTransactionsFailTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProductStockTransactions_ShouldFail
        (Guid id, PaginationRequest paginationRequest, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);

        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProductStockTransactions(id, paginationRequest);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProducts_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProducts(paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsPurchasesAndSalesSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetSalesByProduct_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest)
    {
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSalesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsPurchasesAndSalesFailTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetSalesByProduct_ShouldFail
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest, Error error)
    {
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSalesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsPurchasesAndSalesSuccessTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetPurchasesByProduct_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest)
    {
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchasesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsPurchasesAndSalesFailTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetPurchasesByProduct_ShouldFail
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, DateRangeRequest  dateRangeRequest, SearchRequest  searchRequest, Error error)
    {
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchasesByProduct(id, paginationRequest, sortRequest, dateRangeRequest, searchRequest);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestsHelpers.GetProductsInventoryFaildTestsData), MemberType = typeof(ProductServiceTestsHelpers))]
    public async Task GetProductInventory_ShouldFail
        (Guid id, Error error)
    {
        var db = DbContextFactory.Create();
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories().Slice(0, 10));
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetProductInventory(id);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);

    }

}
