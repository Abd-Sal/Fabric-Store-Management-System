namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemRequest(
    Guid ProductID,
    float Quantity,
    decimal UnitCost
);
