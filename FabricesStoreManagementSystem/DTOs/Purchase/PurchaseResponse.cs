namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseResponse(
    Guid Id,
    string InvoiceNumber,
    SupplierResponse Supplier,
    int ProductsCount,
    decimal TotalAmount,
    decimal PaidAmount,
    PayStatuses Status,
    DateTime CreatedAt,
    List<PurchaseItemsResponse>? PurchaseItems
);