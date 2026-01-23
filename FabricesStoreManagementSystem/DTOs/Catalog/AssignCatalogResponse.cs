namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record AssignCatalogResponse(
    Guid Id,
    Guid CustomerID,
    Guid CatalogID,
    DateTime AssignedAt,
    DateTime? ReturnedAt
);