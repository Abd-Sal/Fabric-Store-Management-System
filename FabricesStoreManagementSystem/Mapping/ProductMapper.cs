namespace FabricesStoreManagementSystem.Mapping;

public static class ProductMapper
{
    public static ProductResponse ToProductResponse(this Product product)
        => new ProductResponse(
                product.Id, product.Name, product.Code,
                product.Color, product.ProductCode, product.Unit,
                product.Material, product.CreatedAt
            );

    public static ProductWithInventoryResponse ToProductWithInventoryResponse(this Product product, decimal? price)
        => new ProductWithInventoryResponse(
                product.ToProductResponse(),
                product.Inventory?.CurrentQuantity ?? 0,
                price ?? 0,
                product.Inventory?.LastUpdateAt
            );

    public static StockTransactionResponse ToStockTransactionResponse(this StockTransaction stockTransaction)
        => new StockTransactionResponse(
                stockTransaction.Id, stockTransaction.ProductID, stockTransaction.QuantityChange,
                stockTransaction.TransactionType, stockTransaction.ReferenceID, stockTransaction.ReferenceType,
                stockTransaction.Note, stockTransaction.CreatedAt
            );
}
