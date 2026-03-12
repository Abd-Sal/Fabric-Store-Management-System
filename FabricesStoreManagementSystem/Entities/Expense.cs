namespace FabricesStoreManagementSystem.Entities;

public class Expense
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Message { get; set; } = string.Empty;
    public decimal DollarPriceInSyr { get; set; }
    public decimal SyrianAmount { get; set; }
    public decimal DollarAmount => Math.Round(SyrianAmount / DollarPriceInSyr, 3, MidpointRounding.AwayFromZero);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
