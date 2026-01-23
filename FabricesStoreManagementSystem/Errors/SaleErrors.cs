namespace FabricesStoreManagementSystem.Errors;

public class SaleErrors
{
    public static Error NoSuccessfulSaleItems =
        new("Sale.NoSuccessfulSaleItems",
            "no any sale items processed saccessfully",
            StatusCodes.Status400BadRequest);

    public static Error NotFound =
        new("Sale.NotFound",
            "not found the sale",
            StatusCodes.Status404NotFound);

    public static Error PaidMoreThanNetTotal =
        new("Sale.PaidMoreThanTotal",
            "customer pay more than net amount",
            StatusCodes.Status400BadRequest);

    public static Error AlreadyPaid =
        new("Sale.AlreadyPaid",
            "this sale is already paid",
            StatusCodes.Status400BadRequest);
}
