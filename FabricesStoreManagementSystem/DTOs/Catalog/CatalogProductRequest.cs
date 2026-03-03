namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogProductRequest(
    Guid ProductID,
    decimal Quantity
);
