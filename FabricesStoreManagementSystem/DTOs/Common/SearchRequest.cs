namespace FabricesStoreManagementSystem.DTOs.Common;

public record SearchRequest(
    string? Search,
    string? SearchColumn
);