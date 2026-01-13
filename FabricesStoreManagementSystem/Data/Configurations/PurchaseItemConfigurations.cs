namespace FabricesStoreManagementSystem.Data.Configurations;

public class PurchaseItemConfigurations : IEntityTypeConfiguration<PurchaseItem>
{
    public void Configure(EntityTypeBuilder<PurchaseItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.HasIndex(x => x.ProductID);

        builder.ToTable("PurchaseItems");
    }
}