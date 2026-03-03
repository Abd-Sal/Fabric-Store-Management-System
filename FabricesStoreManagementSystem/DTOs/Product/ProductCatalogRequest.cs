namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductCatalogRequest(
    Guid Id,
    decimal Quantity
);