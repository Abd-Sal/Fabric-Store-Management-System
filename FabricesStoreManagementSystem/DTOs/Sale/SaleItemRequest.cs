namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemRequest(
    Guid ProductID,
    float Quantity,
    decimal UnitPrice
);
