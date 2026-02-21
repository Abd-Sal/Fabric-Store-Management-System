namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseResponse(
    Guid Id,
    string InvoiceNumber,
    int ProductsCount,
    decimal TotalAmount,
    decimal PaidAmount,
    PayStatuses Status,
    Guid SupplierID,
    string SupplierName,
    DateTime CreatedAt,
    List<PurchaseItemResponse>? PurchaseItems
);