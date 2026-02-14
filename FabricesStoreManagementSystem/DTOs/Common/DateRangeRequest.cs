namespace FabricesStoreManagementSystem.DTOs.Common;

public record DateRangeRequest(
    DateOnly? From,
    DateOnly? To,
    string Timezone = "Arab Standard Time"
);