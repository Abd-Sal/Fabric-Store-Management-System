namespace FabricesStoreManagementSystem.Data.Configurations;

public class CatalogAssignConfigurations : IEntityTypeConfiguration<CatalogAssign>
{
    public void Configure(EntityTypeBuilder<CatalogAssign> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => new {x.CatalogID, x.CustomerID});
        builder.HasIndex(x => x.CatalogID);
        builder.HasIndex(x => x.CustomerID);

        builder.ToTable("CatalogsAssigns");
    }
}
