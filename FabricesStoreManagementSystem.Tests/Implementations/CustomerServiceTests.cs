namespace FabricesStoreManagementSystem.Tests.Implementations;

public class CustomerServiceTests
{
    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerCreateTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task CreateCustomer_ShouldSccess_WhenOneOrMoreParamIsAcceptNull
        (CustomerRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);
        
        //Act
        var result = await service.CreateCustomer(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var customer = await db.Customers.SingleOrDefaultAsync();
        customer.Should().NotBe(null);
        customer.FirstName.Should().Be("Abd");
        customer.Email.Should().Be(request.Email);
        customer.Phone.Should().Be(request.Phone);
        customer.Address.Should().Be(request.Address);
        customer.IsActive.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerAddTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task CreateCustomer_ShouldFail_WhenEmailOrPhoneDuplicate
        (CustomerRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddAsync(CustomersRepo.Customers()[0]);
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateCustomer(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var customer = await db.Customers.CountAsync();
        customer.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerSuccessTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetCustomer_ShouldSuccess_WithIncludingActiveAndNotActiveCustomer
        (Guid id, bool includeOnlyActive)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddAsync(CustomersRepo.Customers().First());
        await db.Customers.AddAsync(CustomersRepo.Customers().Last());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomer(id, includeOnlyActive);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.IsActive.Should().Be(includeOnlyActive);
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerFailTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetCustomer_ShouldFail_NotFoundWithIncludingActiveAndNotActiveCustomer
        (Guid id, bool includeOnlyActive, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddAsync(CustomersRepo.Customers().First());
        await db.Customers.AddAsync(CustomersRepo.Customers().Last());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomer(id, includeOnlyActive);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetCustomers_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest  searchRequest, bool includeOnlyActive)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomers(paginationRequest, sortRequest, searchRequest, includeOnlyActive);

        //Assert
        result.IsSuccess.Should().BeTrue();
        //result.Error.Code.Should().Be(errCode);
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerUpdateTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task UpdateCustomer_ShouldSuccess
        (Guid id, CustomerRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddAsync(CustomersRepo.Customers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateCustomer(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var updatedCustomer = await db.Customers.FindAsync(id);
        updatedCustomer.Should().NotBeNull();
        updatedCustomer.Email.Should().Be(request.Email);
        updatedCustomer.Phone.Should().Be(request.Phone);
        updatedCustomer.Address.Should().Be(request.Address);
        updatedCustomer.FirstName.Should().Be(request.FirstName);
        updatedCustomer.LastName.Should().Be(request.LastName);
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerUpdateFailTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task UpdateCustomer_ShouldFail_DuplicateEmailOrPhone
        (Guid id, CustomerRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddRangeAsync(CustomersRepo.Customers().Slice(0, 2));
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateCustomer(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var updatedCustomer = await db.Customers.FindAsync(id);
        updatedCustomer.Should().NotBeNull();
        updatedCustomer.Email.Should().Be(CustomersRepo.Customers()[1].Email);
        updatedCustomer.Phone.Should().Be(CustomersRepo.Customers()[1].Phone);
    }

    [Fact]
    public async Task UpdateCustomer_ShouldFail_NotFound()
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        var id = Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07");
        var customers = CustomersRepo.Customers().Slice(0, 2);
        await db.Customers.AddRangeAsync(customers);
        await db.SaveChangesAsync();
        var request = new CustomerRequest("Abd", "Sal", null, null, null);
        //Act
        var result = await service.UpdateCustomer(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CustomerErrors.NotFound);
        var updatedCustomer = await db.Customers.FindAsync(id);
        updatedCustomer.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerToggleTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task ToggleCustomerStatus_ShouldSuccess
        (Guid id, bool? state)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddRangeAsync(CustomersRepo.Customers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.ToggleCustomerStatus(id, state);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var updatedCustomer = await db.Customers.FindAsync(id);
        updatedCustomer.Should().NotBeNull();
        updatedCustomer.IsActive.Should().Be(state != null ? (bool)state : !CustomersRepo.Customers().First().IsActive);
    }

    [Fact]
    public async Task ToggleCustomer_ShouldFail_NotFound()
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        var id = Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07");
        var customers = CustomersRepo.Customers().Slice(0, 2);
        await db.Customers.AddRangeAsync(customers);
        await db.SaveChangesAsync();

        //Act
        var result = await service.ToggleCustomerStatus(id, true);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CustomerErrors.NotFound);
        var updatedCustomer = await db.Customers.FindAsync(id);
        updatedCustomer.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerSalesSuccessTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetSalesByCustomer_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest, SearchInvoiceNumberRequest searchRequest, DateRangeRequest dateRangeRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);

        await db.Customers.AddRangeAsync(CustomersRepo.Customers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSalesByCustomer(id, paginationRequest, searchRequest, dateRangeRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("abd")]
    [InlineData("abd sal")]
    [InlineData(" ")]
    public async Task GetCustomerForBill_ShouldSuccess
        (string name)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomerForBill(new CustomerSearchForBillRequest(name));

        //Assert
        result.IsSuccess.Should().BeTrue();

    }

    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerCatalogsSuccessTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetCustomerCatalogs_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest, SearchCatalogByCodeRequest searchCatalogByCodeRequest, DateRangeRequest dateRangeRequest, bool includeReturned = false)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomerCatalogs(id, paginationRequest, searchCatalogByCodeRequest, dateRangeRequest, includeReturned);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
    
    [Theory]
    [MemberData(nameof(CustomerServiceTestsHelpers.GetCustomerCatalogsFailTestData), MemberType = typeof(CustomerServiceTestsHelpers))]
    public async Task GetCustomerCatalogs_ShouldFail
        (Guid id, PaginationRequest paginationRequest, SearchCatalogByCodeRequest searchCatalogByCodeRequest, DateRangeRequest dateRangeRequest, bool includeReturned, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<CustomerService>.Instance;
        var service = new CustomerService(db, logger);
        await db.Products.AddRangeAsync(ProductsRepo.Products());
        await db.Inventory.AddRangeAsync(ProductInventoriesRepo.Inventories());
        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.Customers.AddRangeAsync(CustomersRepo.Customers());
        await db.Catalogs.AddRangeAsync(CatalogsRepo.Catalogs());
        await db.CatalogsProducts.AddRangeAsync(CatalogProductsRepo.CatalogProducts());
        await db.CatalogsAssigns.AddRangeAsync(CatalogAssignsRepo.CatalogAssings());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetCustomerCatalogs(id, paginationRequest, searchCatalogByCodeRequest, dateRangeRequest, includeReturned);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }
}

