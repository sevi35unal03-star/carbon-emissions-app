namespace IzTek.Carbon.Footprint.Persistence;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder ConfigurePersistence(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.ConfigureServices();

        return builder;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();

        var appDbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await appDbContext.Database.MigrateAsync();
    }
}