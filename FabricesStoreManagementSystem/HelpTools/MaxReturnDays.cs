namespace FabricesStoreManagementSystem.HelpTools;

public class MaxReturnDays
{
    public const string sectionName = "MaxReturnDateDays";
    [Required]
    public int Days { get; set; }
}