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
            _ => sale => sale.CreatedAt
        };

    public static SortColumnsResponse SaleSortColumns()
        => new SortColumnsResponse(
                [new LabelValue("رقم الفاتورة", "invoicenumber"), new LabelValue("عدد المنتجات", "productscount"),
                new LabelValue("صافي القيمة", "netamount"), new LabelValue("المبلغ المدفوع", "paidamount"),
                new LabelValue("الحالة", "status"), new LabelValue("تاريخ الانشاء", "createdat"),
                new LabelValue("قيمة الفاتورة", "totalamount"), new LabelValue("المعرف", "id"),
                new LabelValue("الخصم", "discount")],
                new LabelValue("تاريخ الانشاء", "createdat")
            );
}
