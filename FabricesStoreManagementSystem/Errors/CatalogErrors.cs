namespace FabricesStoreManagementSystem.Errors;

public class CatalogErrors
{
    public static Error NotFound =
        new("Catalog.NotFound",
            "not found the catalog",
            StatusCodes.Status404NotFound);

    public static Error UnavailableCatalog =
        new("Catalog.UnavailableCatalog",
            "this catalog is not available",
            StatusCodes.Status400BadRequest);

    public static Error NotFoundAssignedCatalog =
        new("Catalog.NotFoundAssignedCatalog",
            "not found assigned catalog",
            StatusCodes.Status404NotFound);

    public static Error NotAssignedCatalog =
        new("Catalog.NotAssignedCatalog",
            "this catalog is not assigned",
            StatusCodes.Status400BadRequest);

    public static Error ProductsNotSameCode =
        new("Catalog.ProductsNotSameCode",
            "one or more product has different code",
            StatusCodes.Status400BadRequest);

    public static Error CatalogAlreadyLost =
        new("Catalog.CatalogAlreadyLost",
            "this catalog is already lost ",
            StatusCodes.Status400BadRequest);

    public static Error UnableToProcessCatalogWhichUnavailable =
        new("Catalog.UnableToProcessCatalogWhichUnavailable",
            "cannot manipulate catalog which not in available status",
            StatusCodes.Status400BadRequest);
}
