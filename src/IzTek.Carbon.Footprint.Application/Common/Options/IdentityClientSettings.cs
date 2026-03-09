namespace IzTek.Carbon.Footprint.Application.Common.Options;

public class IdentityClientSettings
{
    public IdentityClient Platform { get; set; } = new();
}

public class IdentityClient
{
    public string ClientId { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}