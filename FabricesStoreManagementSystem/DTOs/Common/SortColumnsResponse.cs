namespace FabricesStoreManagementSystem.DTOs.Common;

public record SortColumnsResponse(
    string[] Columns,
    string Default,
    string DefaultSortDirection = "desc"
);
