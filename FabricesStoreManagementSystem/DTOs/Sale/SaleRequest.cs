namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleRequest(
    Guid CustomerID,
    decimal Discount,
    decimal PaidAmount,
    List<SaleItemRequest> SaleItems
);
