namespace FabricesStoreManagementSystem.Errors;

public class ProductErrors
{
    public static Error NotFound =
        new("Product.NotFound",
            "product not found",
            StatusCodes.Status404NotFound);

    public static Error NotFoundID =
        new("Product.NotFoundID",
            $"one or more product id not found",
            StatusCodes.Status404NotFound);

    public static Error DuplicatedInInvoice =
        new("Product.DuplicatedInInvoice",
            "there is product duplicated in invoice",
            StatusCodes.Status409Conflict);

    public static Error DuplicatedInCatalog =
        new("Product.DuplicatedInCatalog",
            "there is product duplicated in catalog",
            StatusCodes.Status409Conflict);

    public static Error NoQuantity =
        new("Product.NoQuantity",
            "there is not quantity of this product",
            StatusCodes.Status400BadRequest);

    public static Error NoEnoughQuantity =
        new("Product.NoEnoughQuantity",
            "there is not enough quantity of this product",
            StatusCodes.Status400BadRequest);

    //public static Error CodeWithColorConflict =
    //    new("Product.CodeWithColorConflict",
    //        "product with this code and this color is already exist",
    //        StatusCodes.Status409Conflict);
    public static Error CodeWithColorConflict(string prdCode, string prdColor) =>
        new("خطأ في المنتج",
            $"المنتج ذو الكود({prdColor}) و اللون({prdCode}) موجود بالفعل",
            StatusCodes.Status409Conflict);
}
