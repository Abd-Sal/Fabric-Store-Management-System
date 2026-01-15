namespace FabricesStoreManagementSystem.Mapping;

public static class PurchaseMapper
{
    public static PurchaseResponse ToPurchaseResponseWithoutItems(this Purchase purchase)
        => new PurchaseResponse(
                purchase.Id, purchase.InvoiceNumber, purchase.ProductsCount,
                purchase.TotalAmount, purchase.PaidAmount, purchase.Status,
                purchase.CreatedAt, null
            );

    public static PurchaseResponse ToPurchaseResponse(this Purchase purchase)
        => new PurchaseResponse(
                purchase.Id, purchase.InvoiceNumber, purchase.ProductsCount,
                purchase.TotalAmount, purchase.PaidAmount, purchase.Status,
                purchase.CreatedAt, purchase.PurchaseItems.ToPurchaseItemsResponse().ToList()
            );
}
