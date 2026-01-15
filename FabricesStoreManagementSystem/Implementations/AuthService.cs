namespace FabricesStoreManagementSystem.Implementations;

public class AuthService(IOptionsMonitor<AuthOptions> authOptions) : IAuthService
{
    private readonly AuthOptions authOptions = authOptions.CurrentValue;

    public async Task<Result<AuthResponse>> Login
        (LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Username != authOptions.Username || request.Password != authOptions.Password)
            return Result.Failure<AuthResponse>(AuthErrors.WrongUsernameOrPassword);

        var result = new AuthResponse(authOptions.Id);
        return Result.Success(result);
    }
}