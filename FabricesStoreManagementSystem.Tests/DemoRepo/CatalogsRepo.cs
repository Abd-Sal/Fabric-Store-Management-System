namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class CatalogsRepo
{
    public static List<Catalog> Catalogs() => new List<Catalog>()
    {
        new Catalog{ Id = Guid.Parse("c899d0eb-1bc1-4d1f-8f39-5e29e732f1b3"), SupplierID = SuppliersRepo.Suppliers()[0].Id, Status = CatalogStatus.Available, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = null },
        new Catalog{ Id = Guid.Parse("13ce016e-0148-4705-8fd0-3247f2b1f4f5"), SupplierID = SuppliersRepo.Suppliers()[0].Id, Status = CatalogStatus.Lost, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-02") },
        new Catalog{ Id = Guid.Parse("33cb5818-f39d-4068-a8f9-0b5a6279fef3"), SupplierID = SuppliersRepo.Suppliers()[0].Id, Status = CatalogStatus.Assigned, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-03")},
        new Catalog{ Id = Guid.Parse("4b945f79-f7f4-458f-8c33-67f3a11efd35"), SupplierID = SuppliersRepo.Suppliers()[0].Id, Status = CatalogStatus.Available, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-04") },

        new Catalog{ Id = Guid.Parse("cd856b1e-5d2b-48f7-93a6-0114f3142851"), Status = CatalogStatus.Available, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = null },
        new Catalog{ Id = Guid.Parse("32070482-6a05-4e96-92ad-b61f30004028"), Status = CatalogStatus.Lost, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-02") },
        new Catalog{ Id = Guid.Parse("77f93273-a0a5-4fb3-995b-29c6c7126c0a"), Status = CatalogStatus.Assigned, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-03")},
        new Catalog{ Id = Guid.Parse("f25f3096-b0a2-4fc9-8aeb-762627218dd0"), Status = CatalogStatus.Available, CatalogCode = "P-001", Description = "Test Catalog", ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-01"), LastUpdateAt = DateTime.Parse("2026-01-04") },
    };
}