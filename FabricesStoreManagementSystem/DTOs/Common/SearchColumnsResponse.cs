namespace FabricesStoreManagementSystem.DTOs.Common;

public record SearchColumnsResponse(
    LabelValue[] Columns,
    LabelValue Default
);