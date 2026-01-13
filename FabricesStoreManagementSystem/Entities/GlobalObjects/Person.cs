namespace FabricesStoreManagementSystem.Entities;

[NotMapped]
public class Person
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
