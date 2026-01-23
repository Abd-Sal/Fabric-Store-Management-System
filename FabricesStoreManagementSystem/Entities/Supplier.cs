namespace FabricesStoreManagementSystem.Entities;

public class Supplier : Person
{
    public string Name { get; set; } = null!;

    public ICollection<Purchase> Suppliers { get; set; } =
        new List<Purchase>();
    public ICollection<Catalog> Catalogs { get; set; } =
        new List<Catalog>();
}
