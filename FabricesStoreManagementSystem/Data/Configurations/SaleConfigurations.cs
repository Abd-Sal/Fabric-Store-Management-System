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

        builder.ToTable("Sales");
    }
}