namespace FabricesStoreManagementSystem.Data.Configurations;

public class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public const int NameMaxLength = 300;
    public const int UnitMaxLength = 30;
    public const int MaterialMaxLength = 150;
    public const int CodeMaxLength = 50;
    public const int ColorMaxLength = 50;

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnType("nvarchar")
            .HasMaxLength(NameMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Unit)
            .HasColumnType("nvarchar")
            .HasMaxLength(UnitMaxLength);

        builder.Property(x => x.Material)
            .HasColumnType("nvarchar")
            .HasMaxLength(MaterialMaxLength)
            .IsRequired(false);

        builder.Property(x => x.Code)
            .HasColumnType("nvarchar")
            .HasMaxLength(CodeMaxLength);

        builder.Property(x => x.Color)
            .HasColumnType("nvarchar")
            .HasMaxLength(CodeMaxLength);

        builder.HasIndex(x => new { x.Code, x.Color })
            .IsUnique();

        builder.ToTable("Products");
    }
}
