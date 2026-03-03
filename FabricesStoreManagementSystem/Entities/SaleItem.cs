namespace FabricesStoreManagementSystem.Entities;

public class SaleItem
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid SaleID { get; set; }
    public Guid ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => UnitPrice * (decimal)Quantity;

    public Product Product { get; set; } = null!;
    public Sale Sale { get; set; } = null!;

}