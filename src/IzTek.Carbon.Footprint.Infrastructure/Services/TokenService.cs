using IzTek.Carbon.Footprint.Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using TokenResponse = IzTek.Carbon.Footprint.Application.Common.Models.TokenResponse;

namespace IzTek.Carbon.Footprint.Infrastructure.Services;

public class TokenService(
    IHttpClientFactory httpClientFactory,
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtOptions,
    IApplicationDbContext context,
    IMemoryCache memoryCache) : ITokenService  // ← ekle
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("token");
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<string> ClientTokenAsync(IdentityClient client, bool force = false)
    {
        var cacheKey = $"m2m_token_{client.ClientId}";

        if (!force && memoryCache.TryGetValue(cacheKey, out string? cached))
            return cached!;

        var disco = await _httpClient.GetDiscoveryDocumentAsync(client.Authority);
        if (disco.IsError) throw new Exception(disco.Error);

        var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = client.ClientId,
            ClientSecret = client.ClientSecret
        });

        if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

        // Token süresinden 60 saniye önce expire et
        memoryCache.Set(
            cacheKey,
            tokenResponse.AccessToken!,
            TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60));

        return tokenResponse.AccessToken!;
    }

    public async Task<TokenResponse> CreateTokenAsync(User user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwt.SecretKey ?? throw new InvalidOperationException("JWT SecretKey is not configured.")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        // Eski refresh token'ları iptal et
        var oldTokens = await context.RefreshTokens
            .Where(x => x.UserId == user.Id && !x.IsRevoked)
            .ToListAsync();
        foreach (var old in oldTokens)
            old.Revoke("New login");

        // Yeni refresh token oluştur
        var refreshToken = new RefreshToken(
            userId: user.Id,
            token: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            expiresAt: DateTime.UtcNow.AddDays(30));

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return new TokenResponse(
            AccessToken: tokenString,
            TokenType: "Bearer",
            ExpiresIn: _jwt.ExpiryMinutes * 60,
            RefreshToken: refreshToken.Token);
    }

    public async Task<TokenResponse?> RefreshAccessTokenAsync(string refreshToken)
    {
        // Atomic update — tek sorguda hem kontrol et hem revoke et
        var affected = await context.RefreshTokens
            .Where(x => x.Token == refreshToken
                     && !x.IsRevoked
                     && x.ExpiresAt > DateTime.UtcNow)
            .ExecuteUpdateAsync(x => x
                .SetProperty(t => t.IsRevoked, true)
                .SetProperty(t => t.RevokedReason, "Refreshed"));

        // 0 satır etkilendiyse token geçersiz veya zaten kullanılmış
        if (affected == 0)
            return null;

        // Token geçerliydi, user'ı getir
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token is null) return null;

        var user = await userManager.FindByIdAsync(token.UserId.ToString());

        if (user is null || user.IsDeleted)
            return null;

        return await CreateTokenAsync(user);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken, string reason = "Logout")
    {
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (token is null) return;

        token.Revoke(reason);
        await context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string reason = "Account deleted")
    {
        var tokens = await context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var t in tokens)
            t.Revoke(reason);

        await context.SaveChangesAsync();
    }
}