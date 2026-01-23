namespace FabricesStoreManagementSystem.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string? Name { get; set; }
    public string Code { get; set; } = null!;
    public string Color { get; set; } = null!;
    public string ProductCode => $"{Code}-{Color}";
    public string Unit { get; set; } = null!;
    public string? Material { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Inventory? Inventory { get; set; }
    public ICollection<PurchaseItem> PurchaseItems { get; set; } =
        new List<PurchaseItem>();
    public ICollection<SaleItem> SaleItems { get; set; } =
        new List<SaleItem>();
    public ICollection<StockTransaction> StockTransactions { get; set; } =
        new List<StockTransaction>();
    public ICollection<CatalogProduct> CatalogsProducts { get; set; } =
        new List<CatalogProduct>();
}

