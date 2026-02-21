namespace FabricesStoreManagementSystem.HelpTools;

public class HelperTools
{
    public static string GenerateInvoiceNumber()
        => DateTime.Now.ToString("yyyyMMddHHmmss");
}
