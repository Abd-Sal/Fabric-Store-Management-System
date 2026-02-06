namespace FabricesStoreManagementSystem.DTOs.Catalog;

public record CatalogFormPurchaseCatalogRequest(
    Guid SupplierID,
    string? Description,
    List<Guid> Items,
    decimal Amount,
    decimal PaidAmount
);