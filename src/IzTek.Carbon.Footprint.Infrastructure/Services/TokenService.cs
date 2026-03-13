using IzTek.Carbon.Footprint.Infrastructure.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class TokenService(
    IHttpClientFactory httpClientFactory,
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtOptions) : ITokenService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("token");
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<string> ClientTokenAsync(IdentityClient client, bool force = false)
    {
        // Mevcut kod — değişmiyor
        using var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = client.Authority
        });
        if (disco.IsError) throw disco.Exception!;

        var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
        {
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret,
            Address = disco.TokenEndpoint,
        };

        using var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);
        if (newToken.IsError) throw newToken.Exception!;

        return newToken.AccessToken!;
    }

    public async Task<TokenResponse> CreateTokenAsync(User user)
    {
        // 1. Kullanıcının rollerini al
        var roles = await userManager.GetRolesAsync(user);

        // 2. Claim'leri oluştur
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new("fullName", $"{user.Name} {user.Surname}"),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token'ı benzersiz yapar
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        // 3. Rolleri claim olarak ekle
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // 4. İmzalama anahtarı
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 5. Token oluştur
        var expiry = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse(
            AccessToken: tokenString,
            TokenType: "Bearer",
            ExpiresIn: _jwt.ExpiryMinutes * 60);  // saniye cinsinden
    }
}