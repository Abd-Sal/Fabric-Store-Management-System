namespace FabricesStoreManagementSystem.SortingConfig;

public class SaleSorts
{
    public static Expression<Func<Sale, object>> SaleResponseSort(SortRequest sortRequest)
        => sortRequest.SortColumn?.ToLower() switch
        {
            "invoicenumber" => sale => sale.InvoiceNumber,
            "productscount" => sale => sale.ProductsCount,
            "createdat" => sale => sale.CreatedAt,
            "status" => sale => sale.Status,
            "paidamount" => sale => sale.PaidAmount,
            "netamount" => sale => sale.NetAmount,
            "totalamount" => sale => sale.TotalAmount,
            "discount" => sale => sale.Discount,
            "id" => sale => sale.Discount,
            _ => sale => sale.Id
        };
}