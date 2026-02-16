namespace FabricesStoreManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateBootstrapLogger();

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.InjectOurServices(builder.Configuration);

        var app = builder.Build();

        // Log application start with ERROR level to make it stand out
        Log.Information("=== APPLICATION STARTING ===");
        Log.Information($"Start Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Log.Information($"Current Directory: {Environment.CurrentDirectory}");
        Log.Information($"Command Line Args: {string.Join(" ", args)}");
        Log.Information($"Machine Name: {Environment.MachineName}");
        Log.Information($"OS Version: {Environment.OSVersion}");
        Log.Information($".NET Version: {Environment.Version}");

        app.UseExceptionHandler();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapOpenApi();
        }

        app.UseHsts();

        app.UseHttpsRedirection();

        app.UseCors();
        
        app.UseStaticFiles();

        app.UseCustomLoginMiddleware();

        app.useCustomLoggingMiddlerware();

        app.UseCustomAuthMiddleware();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
