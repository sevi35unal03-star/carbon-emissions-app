using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IzTek.Carbon.Footprint.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class TokenService(
    IHttpClientFactory httpClientFactory,
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("token");
    private readonly JwtSettings _jwt = jwtOptions.Value;

    // --- Mevcut M2M Akışı ---
    public async Task<string> ClientTokenAsync(IdentityClient client, bool force = false)
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(client.Authority);
        if (disco.IsError) throw new Exception(disco.Error);

        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret
        });

        if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);
        return tokenResponse.AccessToken!;
    }
    public async Task<TokenResponse> CreateTokenAsync(User user)
    {
        // 1. Kullanıcı rollerini ve Claim'leri hazırla
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()), //
        new(ClaimTypes.Email, user.Email ?? ""),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 2. İmzalama anahtarını JwtSettings'ten al
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Token nesnesini oluştur
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: creds
        );

        // 4. KRİTİK ADIM: Token'ı string'e dönüştür (Hata burada çıkıyordu)
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // 5. Kendi Record'unu parantez içinde döndür
        return new TokenResponse(
            AccessToken: tokenString,
            TokenType: "Bearer",
            ExpiresIn: _jwt.ExpiryMinutes * 60
        );
    }
}