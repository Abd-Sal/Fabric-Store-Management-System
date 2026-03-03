namespace FabricesStoreManagementSystem.Entities;

public class Inventory
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProductID { get; set; }
    public decimal CurrentQuantity { get; set; }
    public DateTime? LastUpdateAt { get; set; }

    public Product Product { get; set; } = null!;
}
