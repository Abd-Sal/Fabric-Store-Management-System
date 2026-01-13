namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductStockTransactionsResponse(
    ProductResponse Product,
    List<StockTransactionResponse> StockTransactions
);
