namespace FabricesStoreManagementSystem.Tests.Helpers;

public class PurchaseServiceTestsHelpers
{
    public static IEnumerable<object[]> GetPurchasePayFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("7f130c68-7fd1-49af-aa40-641e77625526"),
            new PurchaseUpdatePaidRequest(50),
            PurchaseErrors.NotFound
        };

        yield return new object[]
        {
            PurchasesRepo.Purchases()[0].Id,
            new PurchaseUpdatePaidRequest(500),
            PurchaseErrors.AlreadyPaid
        };

        yield return new object[]
        {
            PurchasesRepo.Purchases()[2].Id,
            new PurchaseUpdatePaidRequest(1500),
            PurchaseErrors.PaidMoreThanTotal
        };

        yield return new object[]
        {
            PurchasesRepo.Purchases()[1].Id,
            new PurchaseUpdatePaidRequest(1500),
            PurchaseErrors.PaidMoreThanTotal
        };
    }

    public static IEnumerable<object[]> GetPurchasePaySuccessTestsData()
    {
        yield return new object[]
        {
            PurchasesRepo.Purchases()[2].Id,
            new PurchaseUpdatePaidRequest(50),
            PayStatuses.NotCompleted
        };

        yield return new object[]
        {
            PurchasesRepo.Purchases()[2].Id,
            new PurchaseUpdatePaidRequest(500),
            PayStatuses.Paid
        };
    }
    
    public static IEnumerable<object[]> GetPurchaseCreateSuccessTestsData()
    {
        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                100,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[0].Id, 2, 100)
                }
            ),
            PayStatuses.NotCompleted
        };

        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                400,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new PurchaseItemRequest(ProductsRepo.Products()[1].Id, 2, 100)
                }
            ),
            PayStatuses.Paid
        };

        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                0,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[2].Id, 2, 100),
                    new PurchaseItemRequest(ProductsRepo.Products()[3].Id, 2, 100)
                }
            ),
            PayStatuses.NotPaid
        };
    }

    public static IEnumerable<object[]> GetPurchaseCreateFailTestsData()
    {
        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                100,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new PurchaseItemRequest(ProductsRepo.Products()[0].Id, 2, 100)
                }
            ),
            ProductErrors.DuplicatedInInvoice
        };

        yield return new object[]
        {
            new PurchaseRequest(
                Guid.Parse("76e57e5c-a79e-4119-9783-cc966b7ad1a9"),
                400,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[0].Id, 2, 100),
                    new PurchaseItemRequest(ProductsRepo.Products()[1].Id, 2, 100)
                }
            ),
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                1000,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(ProductsRepo.Products()[2].Id, 2, 100),
                    new PurchaseItemRequest(ProductsRepo.Products()[3].Id, 2, 100)
                }
            ),
            PurchaseErrors.PaidMoreThanTotal
        };

        yield return new object[]
        {
            new PurchaseRequest(
                SuppliersRepo.Suppliers().First().Id,
                1000,
                new List<PurchaseItemRequest>
                {
                    new PurchaseItemRequest(Guid.Parse("353ae2d1-ce1e-49b9-82d2-ea0dc1b11860"), 2, 100),
                    new PurchaseItemRequest(Guid.Parse("ba1d87b8-4de6-4d0b-98c0-d717e160ab56"), 2, 100)
                }
            ),
            ProductErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetPurchasesSuccessTestsData()
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
            new SearchRequest("supplierid", "4ea366b9-0c89-4b90-ab50-e071d7ad20fc")
        };
    }

    public static IEnumerable<object[]> GetPurchaseSuccessTestsData()
    {
        yield return new object[]
        {
            PurchasesRepo.Purchases()[0].Id
        };
        yield return new object[]
        {
            PurchasesRepo.Purchases()[1].Id
        };
    }

    public static IEnumerable<object[]> GetPurchaseFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("0825d49d-15c6-48ec-add8-a7cbea41bbfa"),
            PurchaseErrors.NotFound
        };
        yield return new object[]
        {
            Guid.Parse("6b532845-c13d-4981-b14c-730405d5fa4e"),
            PurchaseErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetPurchaseRemoveSuccessTestsData()
    {
        yield return new object[]
        {
            PurchasesRepo.Purchases()[0].Id
        };
        yield return new object[]
        {
            PurchasesRepo.Purchases()[1].Id
        };
    }

    public static IEnumerable<object[]> GetPurchaseRemoveFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("293f5947-94a9-4df1-a8ff-439200696491"),
            PurchaseErrors.NotFound
        };
        yield return new object[]
        {
            PurchasesRepo.Purchases()[2].Id,
            PurchaseErrors.UnableToReturnPurchase
        };
    }

}
