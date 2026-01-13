namespace FabricesStoreManagementSystem.DTOs.Supplier;

public record SupplierResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime JoinDate
);