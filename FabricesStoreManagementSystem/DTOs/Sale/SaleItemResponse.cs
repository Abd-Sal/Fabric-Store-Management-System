namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemResponse(
    Guid Id,
    Guid ProductID,
    string ProductURL,
    float Quantity,
    decimal UnitPrice,
    decimal Total
);