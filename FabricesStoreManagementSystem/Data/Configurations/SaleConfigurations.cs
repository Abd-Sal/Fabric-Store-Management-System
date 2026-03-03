namespace FabricesStoreManagementSystem.Data.Configurations;

public class SaleConfigurations : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
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

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 3);

        builder.Property(x => x.PaidAmount)
            .HasPrecision(18, 3);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 3);

        builder.Property(x => x.Discount)
            .HasPrecision(18, 3);

        builder.ToTable("Sales");
    }
}