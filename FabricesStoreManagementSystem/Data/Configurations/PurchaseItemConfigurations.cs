namespace FabricesStoreManagementSystem.Data.Configurations;

public class PurchaseItemConfigurations : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 3);
        
        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.ProductID);

        builder.ToTable("PurchaseItems");
    }
}