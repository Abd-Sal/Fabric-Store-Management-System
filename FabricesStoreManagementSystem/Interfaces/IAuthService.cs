namespace FabricesStoreManagementSystem.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponse>> Login(LoginRequest request, HttpContext context, CancellationToken cancellationToken = default);
    Task<Result> Logout(HttpContext context);
    Task<Result<AuthResponse>> Verify(HttpContext context);
}
