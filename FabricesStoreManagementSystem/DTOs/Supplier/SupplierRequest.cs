namespace FabricesStoreManagementSystem.DTOs.Supplier;

public record SupplierRequest(
    string Name,
    string? Email,
    string? Phone,
    string? Address
);
