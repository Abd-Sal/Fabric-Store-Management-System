namespace FabricesStoreManagementSystem.CustomMiddlewares;

public class AuthMiddleware(
        RequestDelegate next,
        IOptionsMonitor<AuthOptions> optionsMonitor
    )
{
    private readonly RequestDelegate _next = next;
    private readonly AuthOptions _optionsMonitor = optionsMonitor.CurrentValue;

    public async Task InvokeAsync(HttpContext context)
    {
        var checkAuth = context.Request.Headers.Authorization.ToString()
            .Contains(_optionsMonitor.Id.ToString());
        if (!checkAuth)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        await _next(context);
    }
}