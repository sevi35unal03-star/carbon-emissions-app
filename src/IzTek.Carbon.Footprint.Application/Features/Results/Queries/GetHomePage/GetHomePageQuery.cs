namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;

/// <summary>
/// Ana Sayfa açılışında çağrılır.
///
/// Flutter akışı:
/// 1. Uygulama açılınca GET /user-results/home çağrılır.
/// 2. TopLeaders[0..2] ana sayfada preview olarak gösterilir.
/// 3. "Detaya Git" tıklanınca GET /user-results/leaderboard?month=X&year=Y çağrılır.
/// 4. GET /daily-activities?status=pending ile bekleyen soru var mı kontrol edilir.
///    Varsa kullanıcıya "cevaplanmamış sorularınız var" uyarısı gösterilir.
/// </summary>
public record GetHomePageQuery;