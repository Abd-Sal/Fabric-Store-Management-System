namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemResponse(
    Guid Id,
    Guid ProductID,
    float Quantity,
    decimal UnitCost,
    decimal Total
);
