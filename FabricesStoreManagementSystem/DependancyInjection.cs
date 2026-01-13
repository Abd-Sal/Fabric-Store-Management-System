using FabricesStoreManagementSystem.Data;
using System.Net.NetworkInformation;

namespace FabricesStoreManagementSystem;

public static class DependancyInjection
{
    public static IServiceCollection InjectOurServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();

        services
            .AddEfCoreConfig(configuration)
            .AddSwaggerConfig();

        return services;
    }
    
    private static IServiceCollection AddEfCoreConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
        return services;
    }

    private static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
