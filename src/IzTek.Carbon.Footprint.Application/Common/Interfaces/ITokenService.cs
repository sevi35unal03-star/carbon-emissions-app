namespace IzTek.Carbon.Footprint.Application.Common.Interfaces;

public interface ITokenService
{
    Task<string> ClientTokenAsync(IdentityClient client, bool force = false);
    Task<TokenResponse> CreateTokenAsync(User user);
}