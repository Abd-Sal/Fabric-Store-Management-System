namespace FabricesStoreManagementSystem.Tests.DemoRepo;

public class ProductStockTransactionsRepo
{
    public static List<StockTransaction> StockTransactions() => new List<StockTransaction>
    {
        new StockTransaction
        {
            Id = Guid.Parse("1890535f-426c-4641-ba02-26a376ce5486"),
            Note = "Test Note",
            ProductID = ProductsRepo.Products().First().Id,
            QuantityChange = -2,
            TransactionType = StockTransactionType.Sample
        },
        new StockTransaction
        {
            Id = Guid.Parse("0ccafc8b-cc17-4880-8a78-10c1ead94a84"),
            Note = "Test Note",
            ProductID = ProductsRepo.Products().First().Id,
            QuantityChange = -2,
            TransactionType = StockTransactionType.Sample
        },
        new StockTransaction
        {
            Id = Guid.Parse("edac22b4-4714-4ea5-ad02-82ce86ab3c6b"),
            Note = "Test Note",
            ProductID = ProductsRepo.Products().Last().Id,
            QuantityChange = -2,
            TransactionType = StockTransactionType.Sample
        },
        new StockTransaction
        {
            Id = Guid.Parse("fa122c41-4ac1-4627-892f-3975bc5b6a16"),
            Note = "Test Note",
            ProductID = ProductsRepo.Products().Last().Id,
            QuantityChange = -2,
            TransactionType = StockTransactionType.Sample
        },
    };
}