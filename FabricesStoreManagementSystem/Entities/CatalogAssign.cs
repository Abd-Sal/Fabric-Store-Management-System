namespace FabricesStoreManagementSystem.Entities;

public class CatalogAssign
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid CatalogID { get; set; }
    public Guid CustomerID { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReturnedAt {  get; set; }

    public Catalog Catalog { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}