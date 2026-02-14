namespace FabricesStoreManagementSystem.DTOs.Common;

public record SortColumnsResponse(
    LabelValue[] Columns,
    LabelValue Default,
    string DefaultSortDirection = "desc"
);