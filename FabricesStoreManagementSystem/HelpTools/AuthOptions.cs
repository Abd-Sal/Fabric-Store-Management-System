namespace FabricesStoreManagementSystem.HelpTools;

public class AuthOptions
{
    public const string sectionName = "Auth";

    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    [Range(1, 43200)]   //30 Days
    public int ExpiresInMinuts { get; set; }
}
