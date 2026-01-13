namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemRequest(
    Guid ProductID,
    float Qunatity,
    decimal UnitPrice
);