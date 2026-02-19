namespace FabricesStoreManagementSystem.DTOs.Customer;

public record CustomerRequest(
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? Address
);