namespace FabricesStoreManagementSystem.DTOs.Purchase;

public record PurchaseRequest(
    Guid SupplierID,
    decimal? PaidAmount,
    List<PurchaseItemRequest> PurchaseItems
);
