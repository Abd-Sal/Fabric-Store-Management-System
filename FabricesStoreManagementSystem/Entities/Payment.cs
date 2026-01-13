namespace FabricesStoreManagementSystem.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid ReferenceID { get; set; }
    public ReferenceTypes ReferenceType { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PayMethod { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}
