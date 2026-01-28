namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class CustomersRepo
{
    public static List<Customer> Customers() => new List<Customer>()
    {
        new Customer { Id = Guid.Parse("7fbd7727-d37d-46ec-b212-8b5a9cac07c0"), FirstName = "Abd", LastName = "Sal", Email = "abd.test.syr@gmail.com", Phone = "0982760361", Address = "Syria Damascus Sahnaya", CreatedAt = DateTime.Parse("2026-01-01"), IsActive = true},
        new Customer { Id = Guid.Parse("1bb6080c-0d6b-4949-bfac-f7e3ab5d8eea"), FirstName = "Abd", LastName = "Sal", Email = null, Phone = null, Address = null, CreatedAt = DateTime.Parse("2026-01-01"), IsActive = true},
        new Customer { Id = Guid.Parse("cfa8c105-0d3f-4acb-885c-d632d8304f2e"), FirstName = "Abd", LastName = "Sal", Email = null, Phone = "0982760361", Address = "Syria Damascus Sahnaya", CreatedAt = DateTime.Parse("2026-01-01"), IsActive = true},
        new Customer { Id = Guid.Parse("ba0f527f-ee5f-4470-992d-1fb19ae5da2d"), FirstName = "Abd", LastName = "Sal", Email = "abd.test.syr@gmail.com", Phone = null, Address = "Syria Damascus Sahnaya", CreatedAt = DateTime.Parse("2026-01-01"), IsActive = true},
        new Customer { Id = Guid.Parse("7eb1b1c0-9cf8-4367-b4d4-0b45d45188e5"), FirstName = "Abd", LastName = "Sal", Email = "abd.test.syr@gmail.com", Phone = "0982760361", Address = null, CreatedAt = DateTime.Parse("2026-01-01"), IsActive = true},
        new Customer { Id = Guid.Parse("8e5bfaa3-1129-4da2-9b3d-7b4509a23394"), FirstName = "Abd", LastName = "Sal", Email = "abd.test.syr@gmail.com", Phone = "0982760361", Address = "Syria Damascus Sahnaya", CreatedAt = DateTime.Parse("2026-01-01"), IsActive = false},
    };
}