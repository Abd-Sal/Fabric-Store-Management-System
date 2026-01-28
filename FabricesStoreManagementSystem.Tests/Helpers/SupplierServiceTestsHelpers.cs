namespace FabricesStoreManagementSystem.Tests.Helpers;

public static class SupplierServiceTestsHelpers
{
    public static IEnumerable<object[]> GetSupplierCreateTestData()
    {
        yield return new object[] { new SupplierRequest("Abd", "abd.test.syr@gmail.com", "0982760361", "Syria Damascus Sahnaya") };
        yield return new object[] { new SupplierRequest("Abd", null, null, null) };
        yield return new object[] { new SupplierRequest("Abd", "abd.test.syr@gmail.com", null, null) };
        yield return new object[] { new SupplierRequest("Abd", "abd.test.syr@gmail.com", "0982760361", null) };
        yield return new object[] { new SupplierRequest("Abd", null, "0982760361", "Syria Damascus Sahnaya") };
        yield return new object[] { new SupplierRequest("Abd", null, null, "Syria Damascus Sahnaya") };
        yield return new object[] { new SupplierRequest("Abd", null, "0982760361", null) };
    }

    public static IEnumerable<object[]> GetSupplierTestData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("name", "asc"),
            null,
            true,
        };

        yield return new object[]
        {
            new PaginationRequest(1, 20),
            new SortRequest("email", "desc"),
            new SearchRequest("abd", "name"),
            true,
        };

        yield return new object[]
        {
            new PaginationRequest { Page = 2, PageSize = 15 },
            new SortRequest("CreatedAt", "ascending"),
            null,
            false,
        };

        yield return new object[]
        {
            new PaginationRequest { Page = 1, PageSize = 5 },
            new SortRequest ("Phone", "DESC"),
            null,
            true,
        };
    }

    public static IEnumerable<object[]> GetSupplierUpdateTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", "0982760361", "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", null, null, null)
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", null, null)
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", "0982760361", null)
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", null, null, "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", null, "0982760361", "Syria-Damascus-Sahnaya"),
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", null, "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@googlemail.com", "0982760360", "Syria-Damascus-Sahnaya")
        };
    }

    public static IEnumerable<object[]> GetSupplierUpdateFailTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers()[1].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", "0982760360", "Syria-Damascus-Sahnaya"),
            SupplierErrors.ConflictEmail
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[1].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@gmail.com", null, "Syria-Damascus-Sahnaya"),
            SupplierErrors.ConflictEmail
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[1].Id,
            new SupplierRequest("Abd Updated", null, "0982760361", "Syria-Damascus-Sahnaya"),
            SupplierErrors.ConflictPhone
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[1].Id,
            new SupplierRequest("Abd Updated", "abd.test.syr@googlemail.com", "0982760361", "Syria-Damascus-Sahnaya"),
            SupplierErrors.ConflictPhone
        };
    }

    public static IEnumerable<object[]> GetSupplierAddTestData()
    {
        yield return new object[]
        {
            new SupplierRequest("Abd", "abd.test.syr@gmail.com", "0982760360", "Syria Damascus Sahnaya"),
            SupplierErrors.ConflictEmail
        };

        yield return new object[]
        {
            new SupplierRequest("Abd", "abd.test.syr@gmail.com", null, "Syria Damascus Sahnaya"),
            SupplierErrors.ConflictEmail
        };

        yield return new object[]
        {
            new SupplierRequest("Abd", "abd.test.syr@googlemail.com", "0982760361", "Syria Damascus Sahnaya"),
            SupplierErrors.ConflictPhone
        };

        yield return new object[]
        {
            new SupplierRequest("Abd", null, "0982760361", "Syria Damascus Sahnaya"),
            SupplierErrors.ConflictPhone
        };
    }

    public static IEnumerable<object[]> GetSupplierToggleTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            true
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            false
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            null
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers()[0].Id,
            null
        };
    }

    public static IEnumerable<object[]> GetSupplierSuccessTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers().Last().Id,
            false
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            true
        };
    }

    public static IEnumerable<object[]> GetSupplierFailTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers().Last().Id,
            true,
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07"),
            true,
            SupplierErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetSupplierSalesSuccessTestData()
    {
        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            new PaginationRequest(1, 10),
            new SortRequest("invoicenumber", "asc"),
            null
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            new PaginationRequest(1, 20),
            new SortRequest("createdat", "desc"),
            new SearchRequest("abd", "status")
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            new PaginationRequest { Page = 2, PageSize = 15 },
            new SortRequest("status", "ascending"),
            null
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            new PaginationRequest { Page = 1, PageSize = 5 },
            new SortRequest ("id", "DESC"),
            null
        };

        yield return new object[]
        {
            SuppliersRepo.Suppliers().First().Id,
            new PaginationRequest { Page = 1, PageSize = 5 },
            new SortRequest ("Unknown", "_DESC"),
            null
        };
    }
}