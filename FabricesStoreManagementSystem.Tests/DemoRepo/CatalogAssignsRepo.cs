namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class CatalogAssignsRepo
{
    public static List<CatalogAssign> CatalogAssings() => new List<CatalogAssign>()
    {
        new CatalogAssign { Id = Guid.Parse("33dc57e9-2109-4f22-b739-34f019d1fb33"), CatalogID = CatalogsRepo.Catalogs()[2].Id, CustomerID = CustomersRepo.Customers()[0].Id, AssignedAt = DateTime.Parse("2026-01-01"), ReturnedAt =  null},
        new CatalogAssign { Id = Guid.Parse("2012085f-2770-462c-a661-7896b22247f6"), CatalogID = CatalogsRepo.Catalogs()[0].Id, CustomerID = CustomersRepo.Customers()[0].Id, AssignedAt = DateTime.Parse("2026-01-01"), ReturnedAt =  DateTime.Parse("2026-01-02")},

        new CatalogAssign { Id = Guid.Parse("a82d2bfe-0ebe-4c55-a6e0-3379258ceabc"), CatalogID = CatalogsRepo.Catalogs()[7].Id, CustomerID = CustomersRepo.Customers()[1].Id, AssignedAt = DateTime.Parse("2026-01-01"), ReturnedAt =  DateTime.Parse("2026-01-04")},
        new CatalogAssign { Id = Guid.Parse("911aab3d-b580-41ea-8198-6480b480cf40"), CatalogID = CatalogsRepo.Catalogs()[6].Id, CustomerID = CustomersRepo.Customers()[1].Id, AssignedAt = DateTime.Parse("2026-01-01"), ReturnedAt =  null}
    };
}
