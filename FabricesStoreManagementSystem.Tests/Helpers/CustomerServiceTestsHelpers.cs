namespace FabricesStoreManagementSystem.Tests.Helpers;

public static class CustomerServiceTestsHelpers
{
    public static IEnumerable<object[]> GetCustomerCreateTestData()
    {
        yield return new object[] { new CustomerRequest("Abd", "Sal", "abd.test.syr@gmail.com", "0982760361", "Syria Damascus Sahnaya") };
        yield return new object[] { new CustomerRequest("Abd", "Sal", null, null, null) };
        yield return new object[] { new CustomerRequest("Abd", "Sal", "abd.test.syr@gmail.com", null, null) };
        yield return new object[] { new CustomerRequest("Abd", "Sal", "abd.test.syr@gmail.com", "0982760361", null) };
        yield return new object[] { new CustomerRequest("Abd", "Sal", null, "0982760361", "Syria Damascus Sahnaya") };
        yield return new object[] { new CustomerRequest("Abd", "Sal", null, null, "Syria Damascus Sahnaya") };
        yield return new object[] { new CustomerRequest("Abd", "Sal", null, "0982760361", null) };
    }

    public static IEnumerable<object[]> GetCustomerTestData()
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

    public static IEnumerable<object[]> GetCustomerUpdateTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", "0982760361", "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", null, null, null)
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", null, null)
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", "0982760361", null)
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", null, null, "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", null, "0982760361", "Syria-Damascus-Sahnaya"),
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", null, "Syria-Damascus-Sahnaya")
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@googlemail.com", "0982760360", "Syria-Damascus-Sahnaya")
        };
    }

    public static IEnumerable<object[]> GetCustomerUpdateFailTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers()[1].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", "0982760360", "Syria-Damascus-Sahnaya"),
            CustomerErrors.ConflictEmail
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[1].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@gmail.com", null, "Syria-Damascus-Sahnaya"),
            CustomerErrors.ConflictEmail
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[1].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", null, "0982760361", "Syria-Damascus-Sahnaya"),
            CustomerErrors.ConflictPhone
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[1].Id,
            new CustomerRequest("Abd Updated", "Sal Updated", "abd.test.syr@googlemail.com", "0982760361", "Syria-Damascus-Sahnaya"),
            CustomerErrors.ConflictPhone
        };
    }

    public static IEnumerable<object[]> GetCustomerAddTestData()
    {
        yield return new object[]
        {
            new CustomerRequest("Abd", "Sal", "abd.test.syr@gmail.com", "0982760360", "Syria Damascus Sahnaya"),
            CustomerErrors.ConflictEmail
        };

        yield return new object[]
        {
            new CustomerRequest("Abd", "Sal", "abd.test.syr@gmail.com", null, "Syria Damascus Sahnaya"),
            CustomerErrors.ConflictEmail
        };

        yield return new object[]
        {
            new CustomerRequest("Abd", "Sal", "abd.test.syr@googlemail.com", "0982760361", "Syria Damascus Sahnaya"),
            CustomerErrors.ConflictPhone
        };

        yield return new object[]
        {
            new CustomerRequest("Abd", "Sal", null, "0982760361", "Syria Damascus Sahnaya"),
            CustomerErrors.ConflictPhone
        };
    }

    public static IEnumerable<object[]> GetCustomerToggleTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            true
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            false
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            null
        };

        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            null
        };
    }

    public static IEnumerable<object[]> GetCustomerSuccessTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers().Last().Id,
            false
        };

        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            true
        };
    }

    public static IEnumerable<object[]> GetCustomerFailTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers().Last().Id,
            true,
            CustomerErrors.NotFound
        };

        yield return new object[]
        {
            Guid.Parse("6acd25f5-9715-4902-a95f-277505cc3b07"),
            true,
            CustomerErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetCustomerSalesSuccessTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            new PaginationRequest(1, 10),
            new SearchInvoiceNumberRequest("202603035502"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
        };

        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            new PaginationRequest(1, 20),
            new SearchInvoiceNumberRequest("202603035502"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
        };

        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            new PaginationRequest { Page = 2, PageSize = 15 },
            new SearchInvoiceNumberRequest("202603035502"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
        };

        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            new PaginationRequest { Page = 1, PageSize = 5 },
            new SearchInvoiceNumberRequest("202603035502"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
        };

        yield return new object[]
        {
            CustomersRepo.Customers().First().Id,
            new PaginationRequest { Page = 1, PageSize = 5 },
            new SearchInvoiceNumberRequest("202603035502"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
        };
    }

    public static IEnumerable<object[]> GetCustomerCatalogsSuccessTestData()
    {
        yield return new object[]
        {
            CustomersRepo.Customers()[0].Id,
            new PaginationRequest(1, 10),
            new SearchCatalogByCodeRequest("5021"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            false
        };
        yield return new object[]
        {
            CustomersRepo.Customers()[3].Id,
            new PaginationRequest(1, 10),
            new SearchCatalogByCodeRequest("AM"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            false
        };
        yield return new object[]
        {
            CustomersRepo.Customers()[1].Id,
            new PaginationRequest(1, 10),
            new SearchCatalogByCodeRequest("P-001"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            true
        };
    }

    public static IEnumerable<object[]> GetCustomerCatalogsFailTestData()
    {
        yield return new object[]
        {
            Guid.Parse("3fb0f743-6b4f-48ce-bcaf-be631f39348e"),
            new PaginationRequest(1, 10),
            new SearchCatalogByCodeRequest("5021"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-02")),
            false,
            CustomerErrors.NotFound,
        };
    }
}
