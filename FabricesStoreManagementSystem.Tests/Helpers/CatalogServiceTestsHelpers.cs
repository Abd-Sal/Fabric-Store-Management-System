namespace FabricesStoreManagementSystem.Tests.Helpers;

public class CatalogServiceTestsHelpers
{
    public static IEnumerable<object[]> GetPayForCatalogFailTestsData()
    {
        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                100,
                100
            ),
            new PurchaseUpdatePaidRequest(100),
            CatalogErrors.AlreadyPaid
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                100,
                50
            ),
            new PurchaseUpdatePaidRequest(150),
            PurchaseErrors.PaidMoreThanTotal
        };

        yield return new object[]
        {
            null,
            new PurchaseUpdatePaidRequest(150),
            CatalogErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetPayForCatalogSuccessTestsData()
    {
        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                100,
                0
            ),
            new PurchaseUpdatePaidRequest(100),
            true
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                100,
                0
            ),
            new PurchaseUpdatePaidRequest(50),
            false
        };
    }

    public static IEnumerable<object[]> GetCatalogPurchaseSuccessTestsData()
    {
        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                52.0m
            ),
            true
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                0m
            ),
            false
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                10m
            ),
            false
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                10m
            ),
            false
        };
    }

    public static IEnumerable<object[]> GetCatalogPurchaseFailTestsData()
    {
        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                Guid.Parse("302b1ef7-ed11-43fc-bbd8-a8b09be22deb"),
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                52.0m
            ),
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[6].Id,
                "Test Description",
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                0m
            ),
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                [Guid.Parse("eccdbf32-6237-4843-aec3-0b743f5b027c"), Guid.Parse("eccdbf32-6237-4843-aec3-0b743f5b027c")],
                52.0m,
                10m
            ),
            ProductErrors.DuplicatedInCatalog
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                [ProductsRepo.Products()[0].Id, Guid.Parse("eccdbf32-6237-4843-aec3-0b743f5b027c")],
                52.0m,
                10m
            ),
            ProductErrors.NotFoundID
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                ProductsRepo.Products().Slice(0, 15).Select(x => x.Id).ToList(),
                52.0m,
                10m
            ),
            CatalogErrors.ProductsNotSameCode
        };

        yield return new object[]
        {
            new CatalogFormPurchaseCatalogRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                ProductsRepo.Products().Slice(0, 5).Select(x => x.Id).ToList(),
                52.0m,
                100m
            ),
            CatalogErrors.PaidMoreThanAmount
        };
    }

    public static IEnumerable<object[]> GetCatalogCreateSuccessTestsData()
    {
        yield return new object[]
        {
            new CatalogRequest(
                "Test Description",
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[3].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[4].Id, 2)
                }
            )
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[3].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[4].Id, 2)
                }
            )
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2)
                }
            )
        };
    }

    public static IEnumerable<object[]> GetCatalogCreateFailTestsData()
    {
        yield return new object[]
        {
            new CatalogRequest(
                "Test Description",
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[3].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[4].Id, 2)
                }
            ),
            ProductErrors.DuplicatedInCatalog
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[3].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[4].Id, 2),
                    new CatalogProductRequest(Guid.Parse("963c0732-7a36-4db8-8036-4e4c7aa3a76a"), 2)
                }
            ),
            ProductErrors.NotFoundID
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[12].Id, 2)
                }
            ),
            CatalogErrors.ProductsNotSameCode
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 12),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[2].Id, 2)
                }
            ),
            ProductErrors.NoEnoughQuantity
        };

        yield return new object[]
        {
            new CatalogRequest(
                null,
                new List<CatalogProductRequest>()
                {
                    new CatalogProductRequest(ProductsRepo.Products()[0].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[9].Id, 2),
                    new CatalogProductRequest(ProductsRepo.Products()[1].Id, 2)
                }
            ),
            ProductErrors.NoQuantity
        };
    }

    public static IEnumerable<object[]> GetCatalogCreateBySupplierSuccessTestsData()
    {
        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[0].Id,
                "Test Description",
                new List<Guid>()
                {
                   ProductsRepo.Products()[0].Id,
                   ProductsRepo.Products()[1].Id,
                   ProductsRepo.Products()[2].Id,
                   ProductsRepo.Products()[3].Id,
                   ProductsRepo.Products()[4].Id
                }
            )
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[1].Id,
                null,
                new List<Guid>()
                {
                   ProductsRepo.Products()[0].Id,
                   ProductsRepo.Products()[1].Id,
                   ProductsRepo.Products()[2].Id,
                   ProductsRepo.Products()[3].Id,
                   ProductsRepo.Products()[4].Id
                }
            )
        };
    }

    public static IEnumerable<object[]> GetCatalogCreateBySupplierFailTestsData()
    {
        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                Guid.Parse("236e37a3-da2f-4f9a-b484-28ca56bbe0e4"),
                "Test Description",
                new List<Guid>()
                {
                    ProductsRepo.Products()[0].Id,
                    ProductsRepo.Products()[1].Id,
                    ProductsRepo.Products()[2].Id,
                    ProductsRepo.Products()[3].Id,
                    ProductsRepo.Products()[4].Id
                }
            ),
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[6].Id,
                "Test Description",
                new List<Guid>()
                {
                    ProductsRepo.Products()[0].Id,
                    ProductsRepo.Products()[1].Id,
                    ProductsRepo.Products()[2].Id,
                    ProductsRepo.Products()[3].Id,
                    ProductsRepo.Products()[4].Id
                }
            ),
            SupplierErrors.NotFound
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                new List<Guid>()
                {
                    ProductsRepo.Products()[1].Id,
                    ProductsRepo.Products()[2].Id,
                    ProductsRepo.Products()[3].Id,
                    ProductsRepo.Products()[4].Id,
                    Guid.Parse("963c0732-7a36-4db8-8036-4e4c7aa3a76a")
                }
            ),
            ProductErrors.NotFoundID
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                new List<Guid>()
                {
                    ProductsRepo.Products()[0].Id,
                    ProductsRepo.Products()[1].Id,
                    ProductsRepo.Products()[12].Id,
                }
            ),
            CatalogErrors.ProductsNotSameCode
        };

        yield return new object[]
        {
            new CatalogFromSupplierRequest(
                SuppliersRepo.Suppliers()[0].Id,
                null,
                new List<Guid>()
                {
                    ProductsRepo.Products()[0].Id,
                    ProductsRepo.Products()[1].Id,
                    ProductsRepo.Products()[1].Id,
                }
            ),
            ProductErrors.DuplicatedInCatalog
        };
    }

    public static IEnumerable<object[]> GetCatalogRemoveSuccessTestsData()
    {
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[0].Id,
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[3].Id,
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[4].Id,
        };
    }

    public static IEnumerable<object[]> GetCatalogRemoveFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("34ce0669-82b1-4f86-91ed-30dbb4553916"),
            CatalogErrors.NotFound
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[1].Id,
            CatalogErrors.UnableToProcessCatalogWhichUnavailable
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[2].Id,
            CatalogErrors.UnableToProcessCatalogWhichUnavailable
        };
    }

    public static IEnumerable<object[]> GetCatalogSuccessTestsData()
    {
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[0].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[1].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[2].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[5].Id
        };
    }

    public static IEnumerable<object[]> GetCatalogFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("f26046cb-5e97-4f80-9927-ad4dc496db4e"),
            CatalogErrors.NotFound
        };
    }

    public static IEnumerable<object[]> GetCatalogsSuccessTestsData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("status", "desc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("status", "asc"),
            null,
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "asc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            null
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "asc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("Lost", "status")
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "asc"),
            null,
            new SearchRequest("Lost", "status")
        };
    }

    public static IEnumerable<object[]> GetCatalogDestructeSuccessTestsData()
    {
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[0].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[2].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[3].Id
        };
    }

    public static IEnumerable<object[]> GetCatalogDestructeFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("6123a318-f302-45b3-8d3d-406976822bc5"),
            CatalogErrors.NotFound
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[1].Id,
            CatalogErrors.CatalogAlreadyLost
        };
    }

    public static IEnumerable<object[]> GetCatalogReturnSuccessTestsData()
    {
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[2].Id
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[6].Id
        };
    }

    public static IEnumerable<object[]> GetCatalogReturnFailTestsData()
    {
        yield return new object[]
        {
            Guid.Parse("e6a470b0-bc6f-44d0-a447-581f769375e2"),
            CatalogErrors.NotFoundAssignedCatalog
        };
        yield return new object[]
        {
            CatalogsRepo.Catalogs()[0].Id,
            CatalogErrors.NotFoundAssignedCatalog
        };
    }

    public static IEnumerable<object[]> GetCatalogAssingSuccessTestsData()
    {
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[0].Id, CatalogsRepo.Catalogs()[0].Id)
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[1].Id, CatalogsRepo.Catalogs()[3].Id)
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[2].Id, CatalogsRepo.Catalogs()[4].Id)
        };
    }

    public static IEnumerable<object[]> GetCatalogAssingFailTestsData()
    {
        yield return new object[]
        {
            new AssignCatalogRequest(Guid.Parse("4cc59e27-27f7-4bfa-b19d-abfe5b1f3123"), CatalogsRepo.Catalogs()[0].Id),
            CustomerErrors.NotFound
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[5].Id, CatalogsRepo.Catalogs()[4].Id),
            CustomerErrors.NotFound
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[1].Id, Guid.Parse("8b252172-8a30-4ecc-8c43-758f54d2dea9")),
            CatalogErrors.NotFound
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[1].Id, CatalogsRepo.Catalogs()[1].Id),
            CatalogErrors.UnavailableCatalog
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[1].Id, CatalogsRepo.Catalogs()[2].Id),
            CatalogErrors.UnavailableCatalog
        };
        yield return new object[]
        {
            new AssignCatalogRequest(CustomersRepo.Customers()[1].Id, CatalogsRepo.Catalogs()[5].Id),
            CatalogErrors.UnavailableCatalog
        };
    }

    public static IEnumerable<object[]> GetAssignedCatalogsSccessTestsData()
    {
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "asc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("Lost", "status"),
            false
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("code", "asc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("Lost", "status"),
            true
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("id", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("Lost", "status"),
            false
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("id", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("5021", "code"),
            false
        };
        yield return new object[]
        {
            new PaginationRequest(1, 10),
            new SortRequest("id", "desc"),
            new DateRangeRequest(DateOnly.Parse("2026-01-02"), DateOnly.Parse("2026-01-04")),
            new SearchRequest("5021", "code"),
            true
        };
    }

    public static IEnumerable<object[]> GetCustomersWhoHasCatalogAndNotBoySccessTestsData()
    {
        yield return new object[]
        {
            1,
            new PaginationRequest(1, 10),
        };
        yield return new object[]
        {
            2,
            new PaginationRequest(2, 20),
        };
    }
}
