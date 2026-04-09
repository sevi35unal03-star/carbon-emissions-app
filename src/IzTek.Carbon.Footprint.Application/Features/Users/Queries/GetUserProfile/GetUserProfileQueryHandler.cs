using Microsoft.AspNetCore.Identity;
namespace IzTek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;

/// <summary>
/// Giriş yapmış kullanıcının profil bilgilerini getirir.
/// 
/// Bu handler aşağıdaki bilgileri üretir:
/// - Kullanıcının kimlik ve kişisel bilgileri (IdentityNumber, Name, Surname, BirthDate)
/// - Kullanıcının sistemde biriktirdiği toplam puan (TotalPoints)
/// - Kullanıcının bugüne kadar bağışladığı toplam ağaç sayısı (DonatedTreeCount)
/// - Mevcut puanına göre bağışlayabileceği maksimum ağaç sayısı (AvailableTreeCount)
/// 
/// AvailableTreeCount değeri, aktif ağaç tanımına (TreeDefinitions) göre
/// kullanıcının toplam puanı üzerinden dinamik olarak hesaplanır.
/// 
/// Bu sorgu genellikle anasayfa veya profil ekranında kullanıcının
/// özet bilgilerini göstermek amacıyla kullanılır.
/// </summary>
public class GetUserProfileQueryHandler(
    UserManager<User> userManager,
    ICurrentUserService currentUserService,
    IApplicationDbContext context)
{
    public async Task<Result<GetUserProfileResponse>> Handle(
        GetUserProfileQuery request,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;
        if (userId is null)
            return Result<GetUserProfileResponse>.Failure(
                SystemErrorCodes.Unauthorized, HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted)
            return Result<GetUserProfileResponse>.Failure(
                SystemErrorCodes.NotFound, HttpStatusCode.NotFound);

        // Aktif ağaç tanımı - kaç ağaç bağışlanabileceğini hesapla
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        var availableTreeCount = treeDef is not null
            ? (int)treeDef.CalculateTreeCount(user.TotalPoints)
            : 0;

        return Result<GetUserProfileResponse>.Success(new GetUserProfileResponse(
            identityNumber: user.IdentityNumber ?? string.Empty,
            name: user.Name,
            surname: user.Surname,
            birthDate: user.BirthDate,
            totalPoints: user.TotalPoints,
            donatedTreeCount: user.DonatedTreeCount,
            availableTreeCount: availableTreeCount
        ));
    }
}