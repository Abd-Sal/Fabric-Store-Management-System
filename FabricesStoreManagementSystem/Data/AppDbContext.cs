using System.Reflection;

namespace FabricesStoreManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
    {
    }
    public DbSet<Customer> Customers{ get; set; }
    public DbSet<Supplier> Suppliers{ get; set; }
    public DbSet<Product> Products{ get; set; }
    public DbSet<Inventory> Inventory{ get; set; }
    public DbSet<Purchase> Purchases{ get; set; }
    public DbSet<PurchaseItem> PurchaseItems{ get; set; }
    public DbSet<Sale> Sales{ get; set; }
    public DbSet<SaleItem> SaleItems{ get; set; }
    public DbSet<Payment> Payments{ get; set; }
    public DbSet<StockTransaction> StockTransactions{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }
}
