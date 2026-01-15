namespace FabricesStoreManagementSystem.Mapping;

public static class SaleItemMapper{
    public static SaleItemResponse TotSaleItemResponse(this SaleItem saleItem)
        => new SaleItemResponse(
                saleItem.Id, saleItem.ProductID,
                saleItem.Quantity, saleItem.UnitPrice,
                saleItem.Total
            );

    public static IEnumerable<SaleItemResponse> TotSaleItemsResponse(this IEnumerable<SaleItem> saleItems)
        => saleItems.Select(x => x.TotSaleItemResponse());
}
