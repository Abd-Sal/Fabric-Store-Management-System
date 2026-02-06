namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogResponse(
    Guid Id,
    string Code,
    string? Description,
    CatalogStatus Status,
    bool IsPurchased,
    decimal? Price,
    decimal? PaidAmount,
    bool? IsPaid,
    DateTime CreatedAt,
    DateTime? LastUpdateAt,
    List<CatalogProductResponse>? Items
);