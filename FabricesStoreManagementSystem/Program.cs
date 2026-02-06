namespace FabricesStoreManagementSystem;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.InjectOurServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapOpenApi();
        }
        app.UseHsts();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.CustomLoginEndpoints();

        app.UseCustomAuthMiddleware();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
