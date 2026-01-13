namespace FabricesStoreManagementSystem.DTOs;

public record DateRangeRequest(
    DateOnly From,
    DateOnly To
);
