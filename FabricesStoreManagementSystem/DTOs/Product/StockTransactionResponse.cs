namespace FabricesStoreManagementSystem.DTOs.Product;

public record StockTransactionResponse(
    Guid Id,
    Guid ProductID,
    float QuantityChange,
    StockTransactionType TransactionType,
    Guid? ReferenceID,
    ReferenceTypes? ReferenceType,
    string? Note,
    DateTime CreatedAt
);