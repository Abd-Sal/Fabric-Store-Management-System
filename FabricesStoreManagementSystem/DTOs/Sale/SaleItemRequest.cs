namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleItemRequest(
    Guid ProductID,
    decimal Quantity,
    decimal UnitPrice
);
