namespace IzTek.Carbon.Footprint.Application.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IHostApplicationBuilder ConfigureApplication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMapster();

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.Configure<IdentityClientSettings>(builder.Configuration.GetSection("IdentityClientSettings"));

        return builder;
    }
}