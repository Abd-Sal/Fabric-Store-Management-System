namespace FabricesStoreManagementSystem.Entities;

public class PurchaseItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid PurchaseID { get; set; }
    public Guid ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Total => UnitCost * (decimal)Quantity;

    public Purchase Purchase { get; set; } = null!;
    public Product Product { get; set; } = null!;
}