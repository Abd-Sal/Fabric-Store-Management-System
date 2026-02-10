namespace FabricesStoreManagementSystem.Tests.Implementations;

public class SupplierServiceTests
{
    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierCreateTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task CreateSupplier_ShouldSccess_WhenOneOrMoreParamIsAcceptNull
        (SupplierRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        //Act
        var result = await service.CreateSupplier(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var Supplier = await db.Suppliers.SingleOrDefaultAsync();
        Supplier.Should().NotBe(null);
        Supplier.Name.Should().Be("Abd");
        Supplier.Email.Should().Be(request.Email);
        Supplier.Phone.Should().Be(request.Phone);
        Supplier.Address.Should().Be(request.Address);
        Supplier.IsActive.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierAddTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task CreateSupplier_ShouldFail_WhenEmailOrPhoneDuplicate
        (SupplierRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers()[0]);
        await db.SaveChangesAsync();

        //Act
        var result = await service.CreateSupplier(request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var Supplier = await db.Suppliers.CountAsync();
        Supplier.Should().Be(1);
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierSuccessTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task GetSupplier_ShouldSuccess_WithIncludingActiveAndNotActiveSupplier
        (Guid id, bool includeOnlyActive)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers().First());
        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers().Last());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSupplier(id, includeOnlyActive);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.IsActive.Should().Be(includeOnlyActive);
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierFailTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task GetSupplier_ShouldFail_NotFoundWithIncludingActiveAndNotActiveSupplier
        (Guid id, bool includeOnlyActive, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers().First());
        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers().Last());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSupplier(id, includeOnlyActive);

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task GetSuppliers_ShouldSuccess
        (PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest  searchRequest, bool includeOnlyActive)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetSuppliers(paginationRequest, sortRequest, searchRequest, includeOnlyActive);

        //Assert
        result.IsSuccess.Should().BeTrue();
        //result.Error.Code.Should().Be(errCode);
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierUpdateTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task UpdateSupplier_ShouldSuccess
        (Guid id, SupplierRequest request)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddAsync(SuppliersRepo.Suppliers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateSupplier(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().NotBeNull();
        updatedSupplier.Email.Should().Be(request.Email);
        updatedSupplier.Phone.Should().Be(request.Phone);
        updatedSupplier.Address.Should().Be(request.Address);
        updatedSupplier.Name.Should().Be(request.Name);
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierUpdateFailTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task UpdateSupplier_ShouldFail_DuplicateEmailOrPhone
        (Guid id, SupplierRequest request, Error error)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers().Slice(0, 2));
        await db.SaveChangesAsync();

        //Act
        var result = await service.UpdateSupplier(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().NotBeNull();
        updatedSupplier.Email.Should().Be(SuppliersRepo.Suppliers()[1].Email);
        updatedSupplier.Phone.Should().Be(SuppliersRepo.Suppliers()[1].Phone);
    }

    [Fact]
    public async Task UpdateSupplier_ShouldFail_NotFound()
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        var id = Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07");
        var Suppliers = SuppliersRepo.Suppliers().Slice(0, 2);
        await db.Suppliers.AddRangeAsync(Suppliers);
        await db.SaveChangesAsync();
        var request = new SupplierRequest("Abd", null, null, null);
        //Act
        var result = await service.UpdateSupplier(id, request);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SupplierErrors.NotFound);
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierToggleTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task ToggleSupplierStatus_ShouldSuccess
        (Guid id, bool? state)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.ToggleSupplierStatus(id, state);
        await db.SaveChangesAsync();

        //Assert
        result.IsSuccess.Should().BeTrue();
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().NotBeNull();
        updatedSupplier.IsActive.Should().Be(state != null ? (bool)state : !SuppliersRepo.Suppliers().First().IsActive);
    }

    [Fact]
    public async Task ToggleSupplier_ShouldFail_NotFound()
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        var id = Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07");
        var Suppliers = SuppliersRepo.Suppliers().Slice(0, 2);
        await db.Suppliers.AddRangeAsync(Suppliers);
        await db.SaveChangesAsync();

        //Act
        var result = await service.ToggleSupplierStatus(id, true);
        await db.SaveChangesAsync();

        //Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SupplierErrors.NotFound);
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(SupplierServiceTestsHelpers.GetSupplierSalesSuccessTestData), MemberType = typeof(SupplierServiceTestsHelpers))]
    public async Task GetSalesBySupplier_ShouldSuccess
        (Guid id, PaginationRequest paginationRequest, SortRequest sortRequest, SearchRequest  searchRequest)
    {
        //Arrange
        var db = DbContextFactory.Create();
        var logger = NullLogger<SupplierService>.Instance;
        var service = new SupplierService(db, logger);

        await db.Suppliers.AddRangeAsync(SuppliersRepo.Suppliers().First());
        await db.SaveChangesAsync();

        //Act
        var result = await service.GetPurchasesBySupplier(id, paginationRequest, sortRequest, searchRequest);

        //Assert
        result.IsSuccess.Should().BeTrue();
        var updatedSupplier = await db.Suppliers.FindAsync(id);
        updatedSupplier.Should().NotBeNull();
    }
}
