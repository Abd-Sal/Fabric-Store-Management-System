namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductResponse(
    Guid Id,
    string? Name,
    string Code,
    string Color,
    string ProductCode,
    string Unit,
    string? Material,
    DateTime CreatedAt
);
