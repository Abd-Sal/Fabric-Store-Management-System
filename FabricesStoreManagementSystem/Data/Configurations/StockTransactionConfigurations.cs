namespace FabricesStoreManagementSystem.Data.Configurations;

public class StockTransactionConfigurations : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Note)
            .HasColumnType("nvarchar")
            .HasMaxLength(500);

        builder.Property(x => x.TransactionType)
            .HasConversion(
                v => v.ToString(),
                v => (StockTransactionType)Enum.Parse(typeof(StockTransactionType), v)
            )
            .HasMaxLength(35);

        builder.Property(x => x.ReferenceType)
            .HasConversion(
                v => v.ToString(),
                v => (ReferenceTypes)Enum.Parse(typeof(ReferenceTypes), v)
            )
            .HasMaxLength(25);

        builder.HasIndex(x => x.ProductID);
        builder.HasIndex(x => x.ReferenceID);
        builder.HasIndex(x => new {x.ReferenceID, x.ReferenceType});

        builder.ToTable("StockTransactions");
    }
}