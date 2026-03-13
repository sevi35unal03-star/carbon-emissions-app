using FluentValidation;
using IzTek.Carbon.Footprint.Application.Common.Behaviors;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Infrastructure.Options;
using IzTek.Carbon.Footprint.Infrastructure.Services;
using IzTek.Carbon.Footprint.Infrastructure.Validators;
using IzTek.Carbon.Footprint.Persistence.Interceptors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using StackExchange.Redis;
using System.Text;
using Wolverine;

using DomainRole = IzTek.Carbon.Footprint.Domain.Entities.Role;


var builder = WebApplication.CreateBuilder(args);

// 1. Wolverine Yapılandırması
builder.Host.UseWolverine(opts =>
{
    // Command/Query handler'larını tara
    opts.Discovery.IncludeAssembly(typeof(LoginCommand).Assembly);
    opts.Policies.AddMiddleware<CachingBehavior>();
    opts.Policies.AddMiddleware<CacheInvalidationBehavior>();

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
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommand>();

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
   await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("Redis")!));


// Cache servisi
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<AuditInterceptor>(); 
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// JWT Settings'i DI'a kaydet
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// JWT Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()!;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero  // Default 5 dk tolerans — kapatıyoruz
        };
    });

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


app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();
app.UseHealthCheckEndpoint();

await app.RunAsync();