namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class SalesRepo
{
    public static List<Sale> Sales() => new List<Sale>
    {
        new Sale{ Id = Guid.Parse("be44317c-a5a4-4e3b-8e32-5f44fdf484c2"), CustomerID = CustomersRepo.Customers()[0].Id, TotalAmount = 100, Discount = 0,  NetAmount = 100, PaidAmount = 100, Status = PayStatuses.Paid, ProductsCount = 1, CreatedAt = DateTime.Parse("2026-01-01"), InvoiceNumber = "20260101"},
        new Sale{ Id = Guid.Parse("0ad03d6d-d5ee-4858-a201-ae5319b8c120"), CustomerID = CustomersRepo.Customers()[0].Id, TotalAmount = 1000, Discount = 900,  NetAmount = 100, PaidAmount = 50, Status = PayStatuses.NotCompleted, ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-02"),  InvoiceNumber = "20260102"},
        new Sale{ Id = Guid.Parse("eda362a7-0937-470c-a523-8c3ca8d68d07"), CustomerID = CustomersRepo.Customers()[0].Id, TotalAmount = 500, Discount = 0,  NetAmount = 100, PaidAmount = 0, Status = PayStatuses.NotPaid, ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-02"),  InvoiceNumber = "20260103"},
        new Sale{ Id = Guid.Parse("5112a702-1117-49bd-8d33-2337c34a14bf"), CustomerID = CustomersRepo.Customers()[0].Id, TotalAmount = 500, Discount = 500,  NetAmount = 0, PaidAmount = 0, Status = PayStatuses.Paid, ProductsCount = 5, CreatedAt = DateTime.Parse("2026-01-02"),  InvoiceNumber = "20260104"},
    };
}
