namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseItemsResponse(
    Guid Id,
    Guid ProductID,
    string ProductURL,
    float Quantity,
    decimal UnitCost,
    decimal Total
);
