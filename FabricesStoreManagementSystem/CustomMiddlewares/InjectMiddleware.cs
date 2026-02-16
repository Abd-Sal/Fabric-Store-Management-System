namespace FabricesStoreManagementSystem.CustomMiddlewares;

public static class InjectMiddleware
{
    public static IApplicationBuilder UseCustomAuthMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<AuthMiddleware>();

    public static IApplicationBuilder UseCustomLoginMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<CustomLoginMiddleware>();

    public static IApplicationBuilder useCustomLoggingMiddlerware(this IApplicationBuilder builder)
        => builder.UseMiddleware<CustomLoggingMiddleware>();
}
