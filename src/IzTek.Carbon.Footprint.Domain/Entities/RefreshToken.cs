namespace IzTek.Carbon.Footprint.Domain.Entities;

public class RefreshToken : BaseAuditableEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public string? RevokedReason { get; private set; }

    private RefreshToken()
    { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    // IsActive yerine farklı isim kullan
    public bool IsValid => !IsRevoked && !IsExpired; // ← IsActive yerine IsValid

    public void Revoke(string reason = "Manual revoke")
    {
        IsRevoked = true;
        RevokedReason = reason;
    }
}