namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductWithInventoryResponse(
    ProductResponse Product,
    decimal CurrentQuantity,
    decimal LastUnitCost,
    DateTime? LastUpdateAt
);
