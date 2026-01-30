namespace FabricesStoreManagementSystem.Tests.Helpers;

public class SaleServiceTestsHelpers
{
    public static IEnumerable<object[]> GetSaleCreateSuccessTestsData()
    {
        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                100,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 2, 100)
                }
            ),
            PayStatuses.NotCompleted
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                400,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[1].Id, 2, 100)
                }
            ),
            PayStatuses.Paid
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                0,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[2].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[3].Id, 2, 100)
                }
            ),
            PayStatuses.NotPaid
        };
    }

    public static IEnumerable<object[]> GetSaleCreateFailTestsData()
    {
        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                100,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 2, 100)
                }
            ),
            ProductErrors.DuplicatedInInvoice
        };

        yield return new object[]
        {
            new SaleRequest(
                Guid.Parse("76e57e5c-a79e-4119-9783-cc966b7ad1a9"),
                0,
                400,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[1].Id, 2, 100)
                }
            ),
            CustomerErrors.NotFound
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                1000,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[2].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[3].Id, 2, 100)
                }
            ),
            SaleErrors.PaidMoreThanNetTotal
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                400,
                100,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[2].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[3].Id, 2, 100)
                }
            ),
            SaleErrors.PaidMoreThanNetTotal
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                1000,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(Guid.Parse("353ae2d1-ce1e-49b9-82d2-ea0dc1b11860"), 2, 100),
                    new SaleItemRequest(Guid.Parse("ba1d87b8-4de6-4d0b-98c0-d717e160ab56"), 2, 100)
                }
            ),
            ProductErrors.NotFound
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                200,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[13].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[14].Id, 2, 100),
                }
            ),
            ProductErrors.NoQuantity
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                200,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[9].Id, 2, 100),
                    new SaleItemRequest(ProductsRepo.Products()[8].Id, 2, 100),
                }
            ),
            ProductErrors.NoQuantity
        };

        yield return new object[]
        {
            new SaleRequest(
                CustomersRepo.Customers().First().Id,
                0,
                200,
                new List<SaleItemRequest>
                {
                    new SaleItemRequest(ProductsRepo.Products()[0].Id, 20, 100),
                    new SaleItemRequest(ProductsRepo.Products()[1].Id, 20, 100),
                }
            ),
            ProductErrors.NoEnoughQuantity
        };
    }

    public static IEnumerable<object[]> GetSalesSuccessTestsData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("productscount", "desc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(5, 5),
            new SortRequest("id", "asc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("status", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("createdat", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-01"), DateOnly.Parse("2026-01-10")),
            new SearchRequest("status", "Paid")
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("invoicenumber", "desc"),
            null,
            new SearchRequest("Customerid", "4ea366b9-0c89-4b90-ab50-e071d7ad20fc")
        };
    }

    public static IEnumerable<object[]> GetSaleSuccessTestsData()
    {
        yield return new object[]
        {
            SalesRepo.Sales()[0].Id
        };
        yield return new object[]
        {
            SalesRepo.Sales()[1].Id
        };
    }

    public static IEnumerable<object[]> GetSaleFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("0825d49d-15c6-48ec-add8-a7cbea41bbfa"),
            SaleErrors.NotFound
        };
        yield return new object[]
        {
            Guid.Parse("6b532845-c13d-4981-b14c-730405d5fa4e"),
            SaleErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetSaleRemoveSuccessTestsData()
    {
        yield return new object[]
        {
            SalesRepo.Sales()[0].Id
        };
        yield return new object[]
        {
            SalesRepo.Sales()[1].Id
        };
    }

    public static IEnumerable<object[]> GetSaleRemoveFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("293f5947-94a9-4df1-a8ff-439200696491"),
            SaleErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetSalePayAmountSuccessTestsData()
    {
        yield return new object[]
        {
            SalesRepo.Sales()[2].Id,
            new SaleUpdatePaidRequest(10),
            PayStatuses.NotCompleted
        };
        yield return new object[]
        {
            SalesRepo.Sales()[2].Id,
            new SaleUpdatePaidRequest(100),
            PayStatuses.Paid
        };
    }

    public static IEnumerable<object[]> GetSalePayAmountFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("d5533bb3-9063-41e4-8838-e031d3caaa5d"),
            new SaleUpdatePaidRequest(10),
            SaleErrors.NotFound
        };

        yield return new object[]
        {
            SalesRepo.Sales()[0].Id,
            new SaleUpdatePaidRequest(100),
            SaleErrors.AlreadyPaid
        };

        yield return new object[]
        {
            SalesRepo.Sales()[2].Id,
            new SaleUpdatePaidRequest(1000),
            SaleErrors.PaidMoreThanNetTotal
        };

        yield return new object[]
        {
            SalesRepo.Sales()[2].Id,
            new SaleUpdatePaidRequest(1000),
            SaleErrors.PaidMoreThanNetTotal
        };
    }

}
