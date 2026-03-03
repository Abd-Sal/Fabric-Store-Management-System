namespace FabricesStoreManagementSystem.Data.Configurations;

public class InventoryConfigurations : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.ProductID)
            .IsUnique();

        builder.Property(x => x.CurrentQuantity)
            .HasPrecision(18, 2);

        builder.ToTable("Inventory");
    }
}