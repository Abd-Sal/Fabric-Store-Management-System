namespace FabricesStoreManagementSystem.Entities;

public class Catalog
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid? SupplierID { get; set; } = null;
    public string CatalogCode { get; set; } = null!;
    public string? Description { get; set; }
    public int ProductsCount { get; set; }
    public bool IsPurchased { get; set; } = false;
    public decimal? Price { get; set; } = null;
    public decimal? PaidAmount { get; set; } = null;
    public bool? IsPaid =>
        Price is not null && PaidAmount is not null
        ? Price == PaidAmount
        : null;
    public CatalogStatus Status { get; set; } = CatalogStatus.Available;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdateAt { get; set; }

    public ICollection<CatalogProduct> CatalogsProducts{ get; set; } =
        new List<CatalogProduct>();
    public CatalogAssign? CatalogAssign { get; set; }
    public Supplier? Supplier { get; set; }
}