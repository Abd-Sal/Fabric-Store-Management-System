namespace FabricesStoreManagementSystem.CustomMiddlewares;

public class CustomLoggingMiddleware(ILogger<CustomLoggingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<CustomLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string body = "";
        if(context.Request.Method != "GET")
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true, 1024, true);
            body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }
        await next(context);

        if(context.Response.StatusCode >= 200 &&  context.Response.StatusCode < 300)
            _logger.LogInformation("request: \n\tmethod: {method}\n\tpath: {path}\n\theaders: {headers}\n\tbody: {body}\n", context.Request.Method, context.Request.Path, context.Request.Headers, body);
    }
}
