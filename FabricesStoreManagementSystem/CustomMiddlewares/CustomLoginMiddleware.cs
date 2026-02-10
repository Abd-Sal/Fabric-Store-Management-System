namespace FabricesStoreManagementSystem.CustomMiddlewares;

public sealed class CustomLoginMiddleware(RequestDelegate next, ILogger<CustomLoginMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<CustomLoginMiddleware> _logger = logger;

    public async Task InvokeAsync(
        HttpContext context,
        IOptions<AuthOptions> authOptions)
    {
        if (context.Request.Path.Equals("/auth", StringComparison.OrdinalIgnoreCase)
            && context.Request.Method == HttpMethods.Post)
        {
            context.Response.ContentType = "application/json";

            LoginRequest? request;

            try
            {
                request = await JsonSerializer.DeserializeAsync<LoginRequest>(
                    context.Request.Body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid request body");
                return;
            }

            if (request is null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            _logger.LogInformation(
                "Login attempt for user {Username}",
                request.Username);

            if (request.Username != authOptions.Value.Username ||
                request.Password != authOptions.Value.Password)
            {
                _logger.LogWarning(
                    "Login failed for user {Username}",
                    request.Username);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            _logger.LogInformation(
                "Login succeeded for user {Username}",
                request.Username);

            var response = new AuthResponse(authOptions.Value.Id);

            context.Response.StatusCode = StatusCodes.Status200OK;
            await JsonSerializer.SerializeAsync(
                context.Response.Body,
                response);

            return; // 🔥 stop pipeline
        }

        await _next(context);
    }
}