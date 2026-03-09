using FluentValidation;
using IzTek.Carbon.Footprint.Application.Common.Behaviors;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Infrastructure.Services;
using IzTek.Carbon.Footprint.Infrastructure.Validators;

using Scalar.AspNetCore;
using StackExchange.Redis;
using Wolverine;

using DomainRole = IzTek.Carbon.Footprint.Domain.Entities.Role;


var builder = WebApplication.CreateBuilder(args);

// 1. Wolverine Yapılandırması
builder.Host.UseWolverine(opts =>
{
    // Command/Query handler'larını tara
    opts.Discovery.IncludeAssembly(typeof(CreateProductCommand).Assembly);
    opts.Policies.AddMiddleware<CachingBehavior>();
    opts.Policies.AddMiddleware<CacheInvalidationBehavior>();

    // Wolverine içindeki mesajlar için Fluent Validation'ı aktif et
    //opts.UseFluentValidation(typeof(CreateProductCommand).Assembly);

});

// 2. OpenTelemetry Yapılandırması
builder.ConfigureOpenTelemetry(cfg =>
{
    cfg.OtlpEndpoint = builder.Configuration["OpenTelemetry:Endpoint"];
    cfg.ServiceName = builder.Configuration["OpenTelemetry:ServiceName"]!;
});

// 3. Katman Servis Kayıtları
builder.ConfigureApi()
    .ConfigureApplication()
    .ConfigurePersistence()
    .ConfigureInfrastructure();

// 4. Fluent Validation Servis Kaydı (Kritik Eklemeler)
// CreateProductCommand'in bulunduğu assembly'deki tüm validatorları otomatik kaydeder.
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommand>();

// 5. Identity ve Auth Yapılandırması
builder.Services
    .AddIdentity<User, DomainRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddPasswordValidator<PasswordValidator>()
    .AddUserValidator<UserValidator>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));


// Cache servisi
builder.Services.AddScoped<ICacheService, CacheService>();


builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// --- Middleware Hattı ---

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.MapOpenApi();
    app.MapScalarApiReference(opts =>
    {
        opts.WithTitle("IzTek Api")
            .WithTheme(ScalarTheme.Kepler)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

await app.InitializeDatabaseAsync();

app.UseHttpsRedirection();
app.UseLocalization();

// Identity API Endpoint'leri
//app.MapGroup("auth").MapIdentityApi<User>();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
app.UseHealthCheckEndpoint();

app.Run();