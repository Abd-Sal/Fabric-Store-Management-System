namespace FabricesStoreManagementSystem;

public static class DependancyInjection
{
    public static IServiceCollection InjectOurServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        services.AddExceptionHandler<GlobalExceptionHandler.GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddOpenApi();

        services
            .AddAuthConfig(configuration)
            .AddEfCoreConfig(configuration)
            .AddSwaggerConfig()
            .AddOptionsServices()
            .AddFluentValidationConfig()
            .AddServices()
            .AdjustCORS(configuration);

        return services;
    }
    
    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptions = configuration.GetSection(AuthOptions.sectionName).Get<AuthOptions>();
        int expireMinuts = 3600;
        if (authOptions is not null)
            expireMinuts = authOptions.ExpiresInMinuts;
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Name = "auth";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(expireMinuts);
                options.SlidingExpiration = true;
                options.LoginPath = "/auth/login";
                options.LogoutPath = "/auth/logout";
            });
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

    private static IServiceCollection AddOptionsServices(this IServiceCollection services)
    {
        services.AddOptions<AuthOptions>()
            .BindConfiguration(AuthOptions.sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }
    
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<CustomLoggingMiddleware>();
        return services;
    }

    private static IServiceCollection AdjustCORS(this IServiceCollection services, IConfiguration configuration)
    {
        var hosts = configuration.GetSection("CORS:AllowedHosts").Get<string[]>();
        var methods = configuration.GetSection("CORS:AllowedMethods").Get<string[]>();
        if (hosts is null || hosts.Length == 0)
        {
            services.AddCors(options => 
                options.AddDefaultPolicy(opt =>
                {
                    opt
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowCredentials();
                })
            );
        }
        else
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins(hosts)
                        .WithMethods(methods ?? new[] { "GET", "POST" })
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }
        return services;
    }
}