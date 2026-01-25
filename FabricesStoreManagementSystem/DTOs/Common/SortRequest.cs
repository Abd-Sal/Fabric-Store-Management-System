namespace FabricesStoreManagementSystem.DTOs.Common;

public record SortRequest(
    string? SortColumn,
    string? SortDir
);
