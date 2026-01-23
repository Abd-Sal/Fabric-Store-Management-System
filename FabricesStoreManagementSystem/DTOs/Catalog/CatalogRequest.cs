namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogRequest(
    string? Description,
    List<CatalogProductRequest> Items
);