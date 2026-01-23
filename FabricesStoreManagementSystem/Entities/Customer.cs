namespace FabricesStoreManagementSystem.Entities;

public class Customer : Person
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public ICollection<Sale> Sales { get; set; } = 
        new List<Sale>();
    public ICollection<CatalogAssign> CatalogsAssigns { get; set; } = 
        new List<CatalogAssign>();
}
