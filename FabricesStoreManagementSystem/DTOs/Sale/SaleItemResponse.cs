namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemResponse(
    Guid Id,
    Guid ProductID,
    float Quantity,
    decimal UnitPrice,
    decimal Total
);