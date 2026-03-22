namespace FabricesStoreManagementSystem.CustomMiddlewares;

public static class InjectMiddleware
{
    public static IApplicationBuilder useCustomLoggingMiddlerware(this IApplicationBuilder builder)
        => builder.UseMiddleware<CustomLoggingMiddleware>();
}
