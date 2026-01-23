namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogProductResponse(
    Guid Id,
    Guid ProductID,
    Guid CatalogID,
    float Quantity,
    bool IsDeducted
);
