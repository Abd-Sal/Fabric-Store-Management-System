namespace FabricesStoreManagementSystem.HelpTools;

public class AuthOptions
{
    public const string sectionName = "Auth";

    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
