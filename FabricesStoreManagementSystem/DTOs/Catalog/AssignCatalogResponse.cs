namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record AssignCatalogResponse(
    Guid Id,
    Guid CustomerID,
    string CustomerName,
    Guid CatalogID,
    string CatalogCode,
    DateTime AssignedAt,
    DateTime? ReturnedAt
);
