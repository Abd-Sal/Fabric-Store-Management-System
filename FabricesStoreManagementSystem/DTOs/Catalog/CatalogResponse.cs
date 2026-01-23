namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogResponse(
    Guid Id,
    string Code,
    string? Description,
    CatalogStatus Status,
    DateTime CreatedAt,
    DateTime? LastUpdateAt,
    List<CatalogProductResponse>? Items
);