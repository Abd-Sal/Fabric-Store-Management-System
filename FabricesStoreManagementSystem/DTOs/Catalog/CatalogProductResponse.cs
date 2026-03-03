namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogProductResponse(
    Guid Id,
    Guid ProductID,
    Guid CatalogID,
    decimal Quantity,
    bool IsDeducted
);
