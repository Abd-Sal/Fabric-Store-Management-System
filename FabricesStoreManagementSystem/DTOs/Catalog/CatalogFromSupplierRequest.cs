namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogFromSupplierRequest(
    Guid SupplierID,
    string? Description,
    List<Guid> Items
);