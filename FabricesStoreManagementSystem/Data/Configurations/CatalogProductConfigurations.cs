namespace FabricesStoreManagementSystem.Data.Configurations;

public class CatalogProductConfigurations : IEntityTypeConfiguration<CatalogProduct>
{
    public void Configure(EntityTypeBuilder<CatalogProduct> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new {x.CatalogID, x.PorductID});

        builder.ToTable("CatalogsProducts");
    }
}