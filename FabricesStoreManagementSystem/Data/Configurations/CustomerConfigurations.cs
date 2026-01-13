namespace FabricesStoreManagementSystem.Data.Configurations;

public class CustomerConfigurations : IEntityTypeConfiguration<Customer>
{
    public const int FirstNameMaxLength = 150;
    public const int LastNameMaxLength = 150;
    public const int AddressMaxLength = 500;
    public const int EmailMaxLength = 300;
    public const int PhoneMaxLength = 20;
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.FirstName)
            .HasColumnType("nvarchar")
            .HasMaxLength(FirstNameMaxLength);

        builder.Property(x => x.LastName)
            .HasColumnType("nvarchar")
            .HasMaxLength(LastNameMaxLength);

        builder.Property(x => x.Address)
            .HasColumnType("nvarchar")
            .HasMaxLength(AddressMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .HasColumnType("varchar")
            .HasMaxLength(EmailMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Phone)
            .HasColumnType("varchar")
            .HasMaxLength(PhoneMaxLength)
            .IsRequired(false);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.Phone)
            .IsUnique();

        builder.ToTable("Customers");
    }
}
