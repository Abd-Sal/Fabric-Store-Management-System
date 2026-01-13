namespace FabricesStoreManagementSystem.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default);
}
