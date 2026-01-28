namespace FabricesStoreManagementSystem.Tests.Helpers;

public class ProductServiceTestsHelpers
{
    public static IEnumerable<object[]> GetCreateProductsSuccessTestsData()
    {
        yield return new object[]
        {
            new ProductRequest("SWAR Fabric", "P-001", "Red", "Meter", "silk"),
        };
        yield return new object[]
        {
            new ProductRequest(null, "P-001", "Red", "Meter", null),
        };
        yield return new object[]
        {
            new ProductRequest("SWAR Fabric", "P-001", "Red", "Meter", null),
        };
        yield return new object[]
        {
            new ProductRequest(null, "P-001", "Red", "Meter", "silk")
        };
    }

    public static IEnumerable<object[]> GetCreateProductsFailTestsData()
    {
        yield return new object[]
        {
            new ProductRequest("SWAR Fabric", "P-001", "Red", "Meter", "silk"),
            ProductErrors.CodeWithColorConflict
        };
        yield return new object[]
        {
            new ProductRequest(null, "P-001", "Green", "Meter", null),
            ProductErrors.CodeWithColorConflict
        };
        yield return new object[]
        {
            new ProductRequest("SWAR Fabric", "P-002", "Red", "Meter", null),
            ProductErrors.CodeWithColorConflict
        };
        yield return new object[]
        {
            new ProductRequest(null, "P-002", "Green", "Meter", "silk"),
            ProductErrors.CodeWithColorConflict
        };
    }

    public static IEnumerable<object[]> GetProductsShouldSuccessTestsData()
    {
        yield return new object[]
        {
            ProductsRepo.Products()[0].Id
        };
        yield return new object[]
        {
            ProductsRepo.Products()[1].Id
        };
        yield return new object[]
        {
            ProductsRepo.Products()[2].Id
        };
        yield return new object[]
        {
            ProductsRepo.Products()[3].Id
        };
        yield return new object[]
        {
            ProductsRepo.Products()[4].Id
        };
    }

    public static IEnumerable<object[]> GetProductsShouldFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("40646fa8-f6aa-4fee-bb34-bf827956c90b"),
            ProductErrors.NotFound
        };
        yield return new object[]
        {
            Guid.Parse("008e63f3-b4dd-4619-8cee-a063fd0736d0"),
            ProductErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetProductsTransactionsSuccessTestsData()
    {
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(1, 10)
        };
        yield return new object[]
        {
            ProductsRepo.Products().Last().Id,
            new PaginationRequest(1, 5)
        };
    }

    public static IEnumerable<object[]> GetProductsTransactionsFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("4867ef2c-6d24-473d-8e4c-1be587ce22b4"),
            new PaginationRequest(1, 10),
            ProductErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetProductsSuccessTestsData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(5, 5),
            new SortRequest("code", "asc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SearchRequest("code", "P-00")
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            null,
            new SearchRequest("code", "01")
        };
    }

    public static IEnumerable<object[]> GetProductsPurchasesAndSalesSuccessTestsData()
    {
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            null,
            null
        };
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(5, 5),
            new SortRequest("code", "asc"),
            null,
            null
        };
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            null
        };
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SearchRequest("code", "P-00")
        };
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            null,
            new SearchRequest("code", "01")
        };
    }
    
    public static IEnumerable<object[]> GetProductsPurchasesAndSalesFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("5309178d-c839-4564-8124-f82f7b0a9759"),
            new PaginationRequest(1, 10),
            new SortRequest("code", "desc"),
            null,
            null,
            ProductErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetProductsInventoryFaildTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("bca64f8b-38a4-488d-97b6-dcb059883fba"),
            ProductErrors.NotFound
        };
        yield return new object[]
        {
            ProductsRepo.Products().Last().Id,
            ProductErrors.NoQuantity
        };
        yield return new object[]
        {
            ProductsRepo.Products().First().Id,
            ProductErrors.NotFound
        };
    }
}
