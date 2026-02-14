namespace FabricesStoreManagementSystem.GlobalExceptionHandler;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment _environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger = logger;
    private readonly IWebHostEnvironment environment = _environment;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception.Message, exception);
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = environment.IsDevelopment()
                       ? exception.Message
                       : "An error occurred while processing your request.",
            Instance = httpContext.Request.Path,
            Type = exception.GetType().Name
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
