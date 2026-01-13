namespace FabricesStoreManagementSystem.Data.Configurations;

class SupplierConfigurations : IEntityTypeConfiguration<Supplier>
{
    public const int NameMaxLength = 150;
    public const int AddressMaxLength = 500;
    public const int EmailMaxLength = 300;
    public const int PhoneMaxLength = 20;
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnType("nvarchar")
            .HasMaxLength(NameMaxLength);

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

        builder.ToTable("Suppliers");
    }
}