namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogProductResponse(
    Guid Id,
    Guid ProductID,
    string ProductCode,
    Guid CatalogID,
    decimal Quantity,
    bool IsDeducted
);
