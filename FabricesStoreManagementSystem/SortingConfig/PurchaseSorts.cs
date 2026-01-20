namespace FabricesStoreManagementSystem.SortingConfig;

public class PurchaseSorts
{
    public static Expression<Func<Purchase, object>> PurchaseResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "invoicenumber" => purchase => purchase.InvoiceNumber,
            "productscount" => purchase => purchase.ProductsCount,
            "totalamount" => purchase => purchase.TotalAmount,
            "paidamount" => purchase => purchase.PaidAmount,
            "status" => purchase => purchase.Status,
            "createdat" => purchase => purchase.CreatedAt,
            "id" => purchase => purchase.Id,
            _ => purchase => purchase.CreatedAt
        };
}