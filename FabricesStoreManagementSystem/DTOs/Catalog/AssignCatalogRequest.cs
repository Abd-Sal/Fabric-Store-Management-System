namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record AssignCatalogRequest(
    Guid CustomerID,
    Guid CatalogID
);
