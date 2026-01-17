namespace FabricesStoreManagementSystem.DTOs.Sale;

public record SaleResponse(
    Guid Id,
    string InvoiceNumber,
    Guid CustomerID,
    int ProductsCount,
    decimal TotalAmount,
    decimal Discount,
    decimal NetAmount,
    decimal PaidAmount,
    PayStatuses Status,
    DateTime CreatedAt,
    List<SaleItemResponse>? SaleItems
);
