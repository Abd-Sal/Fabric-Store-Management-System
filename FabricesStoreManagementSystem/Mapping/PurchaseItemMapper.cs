namespace FabricesStoreManagementSystem.Mapping;

public static class PurchaseItemMapper
{
    public static PurchaseItemResponse ToPurchaseItemResponse(this PurchaseItem purchaseItem)
        => new PurchaseItemResponse(
                purchaseItem.Id, purchaseItem.ProductID,
                purchaseItem.Product.ProductCode, purchaseItem.Quantity,
                purchaseItem.UnitCost, purchaseItem.Total
            );
    public static IEnumerable<PurchaseItemResponse> ToPurchaseItemsResponse(this IEnumerable<PurchaseItem> purchaseItems)
        => purchaseItems.Select(x => x.ToPurchaseItemResponse());
}