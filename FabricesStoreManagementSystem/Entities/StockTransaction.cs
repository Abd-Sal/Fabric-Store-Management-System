namespace FabricesStoreManagementSystem.Entities;

public class StockTransaction
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProductID { get; set; }
    public float QuantityChange { get; set; }
    public StockTransactionType TransactionType { get; set; }
    public Guid? ReferenceID { get; set; }
    public ReferenceTypes? ReferenceType { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }

    public Product Product { get; set; } = null!;
}
