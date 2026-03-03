namespace FabricesStoreManagementSystem.Data.Configurations;

public class PurchaseConfigurations : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => (PayStatuses)Enum.Parse(typeof(PayStatuses), v)
            )
            .HasMaxLength(25);

        builder.Property(x => x.PaidAmount)
            .HasPrecision(18, 3);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 3);

        builder.HasIndex(x => x.SupplierID);
        builder.HasIndex(x => x.InvoiceNumber).IsUnique();

        builder.ToTable("Purchases");
    }
}
