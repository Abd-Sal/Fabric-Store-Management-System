namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductWithInventoryResponse(
    ProductResponse Product,
    float CurrentQuantity,
    decimal LastUnitCost,
    DateTime? LastUpdateAt
);
