namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemResponse(
    Guid Id,
    Guid ProductID,
    decimal Quantity,
    decimal UnitCost,
    decimal Total
);
