namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemResponse(
    Guid Id,
    Guid ProductID,
    decimal Quantity,
    decimal UnitPrice,
    decimal Total
);