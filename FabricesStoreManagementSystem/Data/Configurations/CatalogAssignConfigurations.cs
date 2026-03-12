namespace FabricesStoreManagementSystem.Data.Configurations;

public class CatalogAssignConfigurations : IEntityTypeConfiguration<CatalogAssign>
{
    public void Configure(EntityTypeBuilder<CatalogAssign> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.ToTable("CatalogsAssigns");
    }
}