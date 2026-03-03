namespace FabricesStoreManagementSystem.Data.Configurations;

public class CatalogProductConfigurations : IEntityTypeConfiguration<CatalogProduct>
{
    public void Configure(EntityTypeBuilder<CatalogProduct> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new {x.CatalogID, x.ProductID});

        builder.ToTable("CatalogsProducts");
    }
}