namespace IzTek.Carbon.Footprint.Infrastructure.Options;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; init; } = null!;
    public string Issuer { get; init; } = null!;
    public string Audience { get; init; } = null!;
    public int ExpiryMinutes { get; init; } = 60;
}