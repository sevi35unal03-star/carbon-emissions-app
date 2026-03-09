namespace IzTek.Carbon.Footprint.Application.Common.Handlers;

public class PlatformClientCredentialTokenHandler(ITokenService tokenService, IOptions<IdentityClientSettings> clientSettings) : DelegatingHandler
{
    private readonly IdentityClientSettings _clientSettings = clientSettings.Value;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenService.ClientTokenAsync(_clientSettings.Platform);

        request.SetBearerToken(token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            token = await tokenService.ClientTokenAsync(_clientSettings.Platform, true);

            request.SetBearerToken(token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}