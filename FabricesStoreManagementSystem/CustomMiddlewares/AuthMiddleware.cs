namespace FabricesStoreManagementSystem.CustomMiddlewares;

public class AuthMiddleware(
        RequestDelegate next,
        IOptionsMonitor<AuthOptions> optionsMonitor,
        ILogger<AuthMiddleware> logger
    )
{
    private readonly RequestDelegate _next = next;
    private readonly AuthOptions _optionsMonitor = optionsMonitor.CurrentValue;
    private readonly ILogger<AuthMiddleware> logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var checkAuth = context.Request.Headers.Authorization.ToString()
            .Contains(_optionsMonitor.Id.ToString());

        if (checkAuth)
        {
            var result = Result.Failure(new Error(
                "Unauthorized",
                "Invalid or expired authorization token",
                StatusCodes.Status401Unauthorized))
                .ToProblem();
            return;
        }
        await _next(context);
    }
}

public static class InjectMiddleware
{
    public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<AuthMiddleware>();
}
