namespace FabricesStoreManagementSystem.Entities;

public class CatalogProduct
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ProductID { get; set; }
    public Guid CatalogID { get; set; }
    public decimal Quantity { get; set; }
    public bool IsDeducted { get; set; } = true;    //is deducted from stock

    public Product Product { get; set; } = null!;
    public Catalog Catalog { get; set; } = null!;
}
