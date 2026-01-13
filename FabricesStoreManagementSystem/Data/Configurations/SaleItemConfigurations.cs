namespace FabricesStoreManagementSystem.Data.Configurations;

public class SaleItemConfigurations : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.SaleID);
        builder.HasIndex(x => x.ProductID);

        builder.ToTable("SaleItems");
    }
}