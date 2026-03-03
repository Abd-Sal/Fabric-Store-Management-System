namespace FabricesStoreManagementSystem.Data.Configurations;

public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.ReferenceType)
            .HasConversion(
                v => v.ToString(),
                v => (ReferenceTypes)Enum.Parse(typeof(ReferenceTypes), v)
            )
            .HasMaxLength(35);

        builder.Property(x => x.PayMethod)
            .HasConversion(
                v => v.ToString(),
                v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v)
            )
            .HasMaxLength(35);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 3);

        builder.HasIndex(x => x.ReferenceID);

        builder.ToTable("Payments");
    }
}