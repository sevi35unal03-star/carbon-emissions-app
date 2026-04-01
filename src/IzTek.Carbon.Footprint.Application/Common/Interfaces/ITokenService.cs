using IzTek.Carbon.Footprint.Application.Common.Models;
using IzTek.Carbon.Footprint.Domain.Entities;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Application.Common.Interfaces; // Namespace'i kontrol et

public interface ITokenService
{
    Task<string> ClientTokenAsync(IdentityClient client, bool force = false);

    Task<TokenResponse> CreateTokenAsync(User user);

    Task<TokenResponse?> RefreshAccessTokenAsync(string refreshToken);

    Task RevokeRefreshTokenAsync(string refreshToken, string reason = "Logout");

    Task RevokeAllUserTokensAsync(Guid userId, string reason = "Account deleted");
}