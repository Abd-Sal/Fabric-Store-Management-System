namespace FabricesStoreManagementSystem.DTOs.Product;

public record ProductRequest(
    string? Name,
    string Code,
    string Color,
    string Unit,
    string? Material
);
