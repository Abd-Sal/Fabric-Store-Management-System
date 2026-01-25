namespace FabricesStoreManagementSystem.DTOs.Common;

public record SearchColumnsResponse(
    string[] Columns,
    string Default
);
