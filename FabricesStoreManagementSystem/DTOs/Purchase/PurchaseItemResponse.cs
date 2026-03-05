namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemResponse(
    Guid Id,
    Guid ProductID,
    string ProductCode,
    decimal Quantity,
    decimal UnitCost,
    decimal Total
);
