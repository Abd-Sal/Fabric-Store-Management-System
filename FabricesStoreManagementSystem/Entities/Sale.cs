namespace FabricesStoreManagementSystem.Entities;

public class Sale
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string InvoiceNumber { get; set; } = Guid.CreateVersion7().ToString();
    public Guid CustomerID { get; set; }
    public int ProductsCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PayStatuses Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
    public ICollection<SaleItem> SaleItems { get; set; } =
        new List<SaleItem>();
}
