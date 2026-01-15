namespace FabricesStoreManagementSystem.Errors;

public class SupplierErrors
{
    public static Error ConflictEmail =
        new("Supplier.ConflictEmail",
            "this email already exist",
            StatusCodes.Status409Conflict);

    public static Error ConflictPhone =
        new("Supplier.ConflictPhone",
            "this phone already exist",
            StatusCodes.Status409Conflict);

    public static Error NotFound =
        new("Supplier.NotFound",
            "not found supplier",
            StatusCodes.Status404NotFound);
}
