namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class TokenService(IHttpClientFactory httpClientFactory) : ITokenService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("token");

    public async Task<string> ClientTokenAsync(IdentityClient client, bool force = false)
    {
        using var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = client.Authority
        });

        if (disco.IsError)
        {
            throw disco.Exception!;
        }

        var clientCredentialTokenRequest = new ClientCredentialsTokenRequest()
        {
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret,
            Address = disco.TokenEndpoint,
        };

        using var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

        if (newToken.IsError)
        {
            throw newToken.Exception!;
        }

        return newToken.AccessToken!;
    }
}