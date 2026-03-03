namespace FabricesStoreManagementSystem.Data.Configurations;

public class SaleItemConfigurations : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 3);

        builder.HasIndex(x => x.SaleID);
        builder.HasIndex(x => x.ProductID);

        builder.ToTable("SaleItems");
    }
}