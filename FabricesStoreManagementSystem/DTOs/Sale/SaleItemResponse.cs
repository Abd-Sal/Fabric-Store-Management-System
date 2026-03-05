namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemResponse(
    Guid Id,
    Guid ProductID,
    string ProductCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total
);