using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Users.Commands.DonateTrees;

// ═══════════════════════════════════════════════════════════════
// SEÇENEk 1: Tüm puan bağışlanır — kullanıcı miktar girmez
// Flutter: POST /users/me/donations (body yok)
// ═══════════════════════════════════════════════════════════════
// public record DonateTreesCommand() : ICacheInvalidator
// {
//     public IEnumerable<string> CacheKeys =>
//     [
//         AppCacheKeys.User.Prefix,
//         AppCacheKeys.Leaderboard.Prefix,
//     ];
// }

// ═══════════════════════════════════════════════════════════════
// SEÇENEk 2: Kısmi bağış — kullanıcı harcamak istediği puanı girer
// Flutter: POST /users/me/donations { "pointsToSpend": 5000 }
// ═══════════════════════════════════════════════════════════════
public record DonateTreesCommand(double PointsToSpend) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.User.Prefix,
        AppCacheKeys.Leaderboard.Prefix,
    ];
}