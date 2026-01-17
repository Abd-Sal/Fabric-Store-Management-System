namespace FabricesStoreManagementSystem.Errors;

public class GeneralErrors
{
    public static Error UnexpectedError =
        new("General.UnexpectedError",
            "unexpected error occur",
            StatusCodes.Status500InternalServerError);
}
