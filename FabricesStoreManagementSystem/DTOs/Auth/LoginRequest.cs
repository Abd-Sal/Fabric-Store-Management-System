namespace FabricesStoreManagementSystem.DTOs.Auth;

public record LoginRequest(
    string Username,
    string Password
);
