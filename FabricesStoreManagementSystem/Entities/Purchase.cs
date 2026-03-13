namespace FabricesStoreManagementSystem.Entities;

public class Purchase : IHasStatus
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string InvoiceNumber { get; set; } = Guid.CreateVersion7().ToString();
    public Guid SupplierID { get; set; }
    public int ProductsCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public PayStatuses Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseItem> PurchaseItems { get; set; } =
        new List<PurchaseItem>();
}
