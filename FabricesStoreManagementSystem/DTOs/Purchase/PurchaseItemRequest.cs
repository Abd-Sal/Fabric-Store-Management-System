namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemRequest(
    Guid ProductID,
    decimal Quantity,
    decimal UnitCost
);