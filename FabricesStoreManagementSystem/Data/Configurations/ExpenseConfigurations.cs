namespace FabricesStoreManagementSystem.Data.Configurations;

public class ExpenseConfigurations : IEntityTypeConfiguration<Expense>
{
    public const int MessageMaxLength = 100;
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Message)
            .HasMaxLength(MessageMaxLength);

        builder.Property(x => x.DollarPriceInSyr)
            .HasPrecision(18, 3);

        builder.Property(x => x.SyrianAmount)
            .HasPrecision(18, 3);

        builder.ToTable("Expenses");
    }
}

