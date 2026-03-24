using IzTek.Carbon.Footprint.Application.Common.Models;
using IzTek.Carbon.Footprint.Domain.Entities;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Application.Common.Interfaces; // Namespace'i kontrol et

public interface ITokenService
{
    Task<string> ClientTokenAsync(IdentityClient client, bool force = false);

    // Eksik olan ve hataya sebep olan metot:
    Task<TokenResponse> CreateTokenAsync(User user);
}