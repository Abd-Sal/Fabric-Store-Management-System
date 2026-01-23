namespace FabricesStoreManagementSystem.Data.Configurations;

public class CatalogConfigurations : IEntityTypeConfiguration<Catalog>
{
    public const int DescriptionMaxLenght = 500;

    public void Configure(EntityTypeBuilder<Catalog> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => (CatalogStatus)Enum.Parse(typeof(CatalogStatus), v)
            )
            .HasColumnType("varchar")
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar")
            .HasMaxLength(CatalogConfigurationDescriptionMaxLenght)
            .IsRequired(false);

        builder.HasIndex(x => x.CatalogCode);
        builder.HasIndex(x => x.SupplierID);

        builder.ToTable("Catalogs");
    }
}