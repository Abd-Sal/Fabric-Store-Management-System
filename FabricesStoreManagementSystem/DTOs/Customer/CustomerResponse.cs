namespace FabricesStoreManagementSystem.DTOs.Customer;

public record CustomerResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Address,
    bool IsActive,
    DateTime JoinDate
);