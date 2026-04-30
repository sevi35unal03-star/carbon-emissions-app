using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using IzTek.Carbon.Footprint.Application.Common.Behaviors;
using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.RefreshTokens.Commands.Cleanup;
using IzTek.Carbon.Footprint.Application.Features.Users.Commands.Login;
using IzTek.Carbon.Footprint.Infrastructure.Options;
using IzTek.Carbon.Footprint.Infrastructure.Services;
using IzTek.Carbon.Footprint.Infrastructure.Validators;
using JasperFx.CodeGeneration;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DomainRole = IzTek.Carbon.Footprint.Domain.Entities.Role;


// Firebase Admin SDK'yi başlat
FirebaseApp.Create(new AppOptions { Credential = GoogleCredential.FromFile("karbon-ai-8d88e-firebase-adminsdk-fbsvc-d0c2986411.json") });

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseLamar();

// 1. Wolverine
builder.Host.UseWolverine(opts =>
{
    opts.Discovery.IncludeAssembly(typeof(LoginCommand).Assembly);
    opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;
    opts.Policies.ForMessagesOfType<object>().AddMiddleware<ValidationBehavior>();
});

// 2. OpenTelemetry
builder.ConfigureOpenTelemetry(cfg =>
{
    cfg.OtlpEndpoint = builder.Configuration["OpenTelemetry:Endpoint"];
    cfg.ServiceName = builder.Configuration["OpenTelemetry:ServiceName"]!;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminPanel", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// 3. Katman servisleri
builder.ConfigureApi()
    .ConfigureApplication()
    .ConfigurePersistence()
    .ConfigureInfrastructure();

// 4. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<LoginCommand>();

// 5. Identity
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

// 7. JWT ayarları
//Local değişkeni dışarı çıkar, bir kez oku
var jwtSettings = builder.Configuration
    .GetSection(JwtSettings.SectionName)
    //jwt de hata olursa uygulama başlamasın, eksik konfigürasyon varsa hemen fark edelim, ekrana yazsın
    .Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings missing");

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));

// 8. Authentication
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        //dışarıdan gelen değişkeni kullan, içeride GetSection kullanımını kaldır.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            //reftoken oluştururken kullanılan secret key
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
// 9. Cache servisi
builder.Services.AddMemoryCache();

// 10. Controllers ve OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

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

try
{
    await app.SeedRolesAsync();
    await app.SeedAdminUserAsync();
    await app.SeedScoringSettingsAsync();
    await app.SeedActivityQuestionsAsync();
    await app.SeedMonthlyPollAsync();
    await app.SeedUsefulInformationsAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Uygulama başlatılırken seed işlemi başarısız oldu.");
    throw;
}

await app.InitializeAssetsAsync();

app.UseHttpsRedirection();
app.UseCors("AdminPanel");
app.UseRateLimiter();
app.UseLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHealthCheckEndpoint();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(async () =>
{
    var bus = app.Services.GetRequiredService<IMessageBus>();
    await bus.ScheduleAsync(
        new CleanupExpiredRefreshTokensCommand(),
        TimeSpan.FromHours(24));
});

await app.RunAsync();