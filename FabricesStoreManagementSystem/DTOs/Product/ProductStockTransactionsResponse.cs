namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductStockTransactionsResponse(
    ProductResponse Product,
    float CurrentQuantity,
    DateTime? LastUpdateAt,
    List<StockTransactionResponse>? StockTransactions
);
