namespace FabricesStoreManagementSystem.Mapping;

public static class SaleMapper
{
    public static SaleResponse ToSaleResponseWithNoItems(this Sale sale)
        => new SaleResponse(
                sale.Id, sale.InvoiceNumber, sale.ProductsCount,
                sale.TotalAmount, sale.Discount,
                sale.NetAmount, sale.PaidAmount, sale.Status,
                sale.CustomerID, $"{sale.Customer?.FirstName} {sale.Customer?.LastName}",
                sale.CreatedAt, null
            );

    public static IEnumerable<SaleResponse> ToSalesResponseWithNoItems(this IEnumerable<Sale> sales)
        => sales.Select(x => x.ToSaleResponseWithNoItems());

    public static SaleResponse ToSaleResponse(this Sale sale)
        => new SaleResponse(
                sale.Id, sale.InvoiceNumber, sale.ProductsCount,
                sale.TotalAmount, sale.Discount,
                sale.NetAmount, sale.PaidAmount, sale.Status,
                sale.CustomerID, $"{sale.Customer.FirstName} {sale.Customer.LastName}",
                sale.CreatedAt, sale.SaleItems.TotSaleItemsResponse().ToList()
            );

    public static IEnumerable<SaleResponse> ToSalesResponse(this IEnumerable<Sale> sales)
        => sales.Select(x => x.ToSaleResponse());
}
