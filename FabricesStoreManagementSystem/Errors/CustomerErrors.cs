namespace FabricesStoreManagementSystem.Errors;

public class CustomerErrors
{
    public static Error ConflictEmail =
        new("Customer.ConflictEmail",
            "this email already exist",
            StatusCodes.Status409Conflict);

    public static Error ConflictPhone =
        new("Customer.ConflictPhone",
            "this phone already exist",
            StatusCodes.Status409Conflict);

    public static Error NotFound =
        new("Customer.NotFound",
            "not found customer",
            StatusCodes.Status404NotFound);
}
