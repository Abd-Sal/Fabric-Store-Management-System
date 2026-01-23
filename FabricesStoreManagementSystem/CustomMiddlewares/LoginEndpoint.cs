namespace FabricesStoreManagementSystem.CustomMiddlewares;

public static class LoginEndpoint
{
    public static RouteHandlerBuilder CustomLoginEndpoints(this IEndpointRouteBuilder app)
    {
        return app.MapPost("/auth",
            (LoginRequest request, IOptions<AuthOptions> authOptions) =>
            {
                if (request.Username != authOptions.Value.Username ||
                    request.Password != authOptions.Value.Password)
                    return Results.Unauthorized();
                return Results.Ok(new AuthResponse(authOptions.Value.Id));
            })
        .Produces<AuthResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}
