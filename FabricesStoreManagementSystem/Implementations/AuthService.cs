namespace FabricesStoreManagementSystem.Implementations;

public class AuthService(IOptionsMonitor<AuthOptions> optionsMonitor) : IAuthService
{
    private readonly AuthOptions _optionsMonitor = optionsMonitor.CurrentValue;

    public async Task<Result<AuthResponse>> Login
        (LoginRequest request, HttpContext context, CancellationToken cancellationToken = default)
    {
        if (request.Username != _optionsMonitor.Username || request.Password != _optionsMonitor.Password)
            return Result.Failure<AuthResponse>(AuthErrors.WrongUsernameOrPassword);

        var token = Guid.NewGuid().ToString();
        var claims = new List<Claim>
        {
            new ("sub", _optionsMonitor.Id.ToString()),
            new ("name", _optionsMonitor.Username),
            new ("key", _optionsMonitor.Username),
            new ("token", token)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(_optionsMonitor.ExpiresInMinuts)
        };

        await context.SignInAsync(
            scheme: CookieAuthenticationDefaults.AuthenticationScheme,
            principal: principal,
            properties: authProperties
        );

        var result = new AuthResponse(token, _optionsMonitor.Username);

        return Result.Success(result);
    }

    public async Task<Result> Logout(HttpContext context)
    {
        await context.SignOutAsync();
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> Verify(HttpContext context)
    {
        var claims = context.User.Claims.Select(x => new { x.Type, x.Value });

        var result = new AuthResponse(
            claims.First(x => x.Type == "token").Value,
            claims.First(x => x.Type == "name").Value
        );

        return Result.Success(result);
    }
}
