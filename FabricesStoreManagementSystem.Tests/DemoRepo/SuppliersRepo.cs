namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class SuppliersRepo
{
    public static List<Supplier> Suppliers() => new List<Supplier>()
    {
        new Supplier { Id = Guid.Parse("885b8eb7-2c91-4ff4-97b3-ecdad04ead53"), Name = "SWAR", Email = "abd.test.syr@gmail.com", Phone = "0982760361", Address = "Syria Damascus Sahnaya", IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("ea764b2e-94d7-40d1-8de7-ee841e11b1e7"), Name = "SWAR", Email = null, Phone = null, Address = null, IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("f5da7a52-1b94-4a55-996b-35eea1cfdbcd"), Name = "SWAR", Email = "abd.test.syr@gmail.com", Phone = null, Address = null, IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("dc9368b3-9988-4974-ab55-092de6191be5"), Name = "SWAR", Email = "abd.test.syr@gmail.com", Phone = "0982760361", Address = null, IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("84cea390-bd23-4600-9a63-98aa41e5cf0f"), Name = "SWAR", Email = null, Phone = null, Address = "Syria Damascus Sahnaya", IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("0562add1-7cfc-49bf-b2b9-31b96da24a5c"), Name = "SWAR", Email = null, Phone = "0982760361", Address = "Syria Damascus Sahnaya", IsActive = true, CreatedAt = DateTime.Parse("2026-01-01")},
        new Supplier { Id = Guid.Parse("8b857d88-e37c-42c7-8138-c7ef54adf6ad"), Name = "SWAR", Email = null, Phone = "0982760361", Address = null, IsActive = false, CreatedAt = DateTime.Parse("2026-01-01")},
    };
}