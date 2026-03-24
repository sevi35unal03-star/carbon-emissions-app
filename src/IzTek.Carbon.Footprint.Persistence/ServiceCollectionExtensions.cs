using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace IzTek.Carbon.Footprint.Persistence;

public static class ServiceCollectionExtensions  // ← class eklendi
{
    public static IHostApplicationBuilder ConfigurePersistence(this IHostApplicationBuilder builder)
    {
        // Interceptor'ları DI'a kaydet
        builder.Services.AddScoped<DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<AuditableEntityInterceptor>();
        builder.Services.AddScoped<AuditInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(
                serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>(),
                serviceProvider.GetRequiredService<AuditableEntityInterceptor>(),
                serviceProvider.GetRequiredService<AuditInterceptor>()
            );
        });

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
            options.InstanceName = "CarbonFootprint:";
        });

        builder.Services.ConfigureServices();
        return builder;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IApplicationDbContext>(
            provider => provider.GetRequiredService<ApplicationDbContext>());
        return services;
    }

    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();
        var appDbContext = serviceScope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        await appDbContext.Database.MigrateAsync();
    }

    public static async Task InitializeAssetsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var fileStorage = scope.ServiceProvider
            .GetRequiredService<IFileStorageService>();

        // Assets bucket'ını public olarak oluştur
        //await fileStorage.EnsureAssetsBucketAsync();
    }

    public static async Task SeedRolesAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<Role>>();

        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new Role { Name = role });
        }
    }

    public static async Task SeedAdminUserAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>().CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        const string adminEmail = "admin@iztek.com";
        const string adminPassword = "Sifre123!";

        var existingUser = await userManager.FindByEmailAsync(adminEmail);

        if (existingUser is not null)
        {
            // Kullanıcı var ama rolü yoksa ekle
            var existingRoles = await userManager.GetRolesAsync(existingUser);
            if (!existingRoles.Contains("Admin"))
                await userManager.AddToRoleAsync(existingUser, "Admin");
            return;
        }

        var adminUser = new User(
            email: adminEmail,
            name: "Admin",
            surname: "User",
            birthDate: DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
            identityNumber: "67890123452",
            phoneNumber: "+905001234567",
            isKvkkApproved: true
        );

        adminUser.ClearDomainEvents();

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin"); // ← Bu satır eksikti
    }
}