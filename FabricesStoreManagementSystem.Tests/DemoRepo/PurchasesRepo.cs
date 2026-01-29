namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class PurchasesRepo
{
    public static List<Purchase> Purchases() => new List<Purchase>
    {
        new Purchase{ Id = Guid.Parse("24f28340-e1b0-453b-962c-a2c66c83170d"), SupplierID = SuppliersRepo.Suppliers()[0].Id, TotalAmount = 100, PaidAmount = 100, Status = PayStatuses.Paid, ProductsCount = 1, CreatedAt = DateTime.Parse("2026-01-01"), InvoiceNumber = "20260101"},
        new Purchase{ Id = Guid.Parse("db445a32-4b0b-4422-9ae5-51e7dc4e385e"), SupplierID = SuppliersRepo.Suppliers()[0].Id, TotalAmount = 1000, PaidAmount = 100, Status = PayStatuses.NotCompleted, ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-02"),  InvoiceNumber = "20260102"},
        new Purchase{ Id = Guid.Parse("c88569d6-57fb-45d0-90f6-f55e02b0fba3"), SupplierID = SuppliersRepo.Suppliers()[0].Id, TotalAmount = 500, PaidAmount = 0, Status = PayStatuses.NotPaid, ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-02"),  InvoiceNumber = "20260103"},
    };
}