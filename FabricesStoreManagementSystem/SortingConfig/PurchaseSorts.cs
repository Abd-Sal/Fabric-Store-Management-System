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

    public static SortColumnsResponse PurchaseSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("رقم الفاتورة", "invoicenumber"), new LabelValue("المبلغ المدفوع", "paidamount"),
                new LabelValue("قيمة الفاتورة", "totalamount"), new LabelValue("عدد المنتجات", "productscount"),
                new LabelValue("تاريخ الانشاء", "createdat"), new LabelValue("المعرف", "id"),
                new LabelValue("الحالة", "status")],
                new LabelValue("تاريخ الانشاء", "createdat")
            );
}