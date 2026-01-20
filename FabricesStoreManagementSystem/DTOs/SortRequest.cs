namespace FabricesStoreManagementSystem.DTOs;

public record SortRequest(
    string? SortColumn,
    string? SortDir
);