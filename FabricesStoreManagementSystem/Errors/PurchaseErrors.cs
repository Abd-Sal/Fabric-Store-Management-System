namespace FabricesStoreManagementSystem.Errors;

public class PurchaseErrors
{
    public static Error NoSuccessfulPurchsaeItems =
        new("Purchase.NoSuccessfulPurchaseItems",
            "no any purchase items processed saccessfully",
            StatusCodes.Status400BadRequest);
    
    public static Error UnableToReturnPurchase =
        new("Purchase.UnableToReturnPurchase",
            "unable to return purchase cause there is one or more sales of this ",
            StatusCodes.Status400BadRequest);

    public static Error NotFound =
        new("Purchase.NotFound",
            "not found the purchase",
            StatusCodes.Status404NotFound);

    public static Error PaidMoreThanTotal =
        new("Purchase.PaidMoreThanTotal",
            "you pay more than total amount",
            StatusCodes.Status400BadRequest);

    public static Error AlreadyPaid =
        new("Purchase.AlreadyPaid",
            "this purchase is already paid",
            StatusCodes.Status400BadRequest);
}
