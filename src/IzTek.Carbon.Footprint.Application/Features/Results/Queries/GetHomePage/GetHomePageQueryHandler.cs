namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetHomePage;

public static class GetHomePageQueryHandler
{
    public static async Task<Result<GetHomePageResponse>> Handle(
    GetHomePageQuery query,
    IApplicationDbContext context,
    ICurrentUserService currentUser,
    CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var currentUserId = currentUser.UserId;

        // Kullanıcı bu ay anketi doldurdu mu?
        var hasCompletedPoll = await context.UserPollResults
            .AnyAsync(x => x.UserId == currentUserId
                        && x.Month == now.Month
                        && x.Year == now.Year, ct);

        // Anketi doldurmamışsa sade ekran döndür
        if (!hasCompletedPoll)
            return Result<GetHomePageResponse>.Success(new GetHomePageResponse(
                HasCompletedPoll: false,
                GlobalTarget: null,
                MonthlyTarget: null,
                TopLeaders: null,
                CurrentUserRank: null));

        // 1. Aktif TreeDefinition
        var treeDef = await context.TreeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive, ct);

        var globalTarget = treeDef?.GlobalTargetTreeCount ?? 0;

        // 2. Tüm zamanlar toplam bağış
        var totalDonatedAllTime = await context.TreeDonations
            .AsNoTracking()
            .SumAsync(x => x.TreeCount, ct);

        // 3. Bu aya ait bağış
        var totalDonatedThisMonth = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.DonationDate.Month == now.Month
                     && x.DonationDate.Year == now.Year)
            .SumAsync(x => x.TreeCount, ct);

        // 4. Bu aya ait global hedef
        var monthlyGoal = await context.Goals
            .AsNoTracking()
            .Where(x => x.Month == now.Month && x.Year == now.Year)
            .Select(x => x.TargetTreeCount)
            .FirstOrDefaultAsync(ct);

        // 5. Ana sayfa liderboard preview
        var donations = await context.TreeDonations
            .AsNoTracking()
            .Where(x => x.DonationDate.Month == now.Month
                     && x.DonationDate.Year == now.Year)
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalTrees = g.Sum(x => x.TreeCount)
            })
            .ToListAsync(ct);

        var userIds = donations.Select(x => x.UserId).ToList();
        var users = await context.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, FullName = u.Name + " " + u.Surname })
            .ToListAsync(ct);

        var allRankingsRaw = donations
            .Join(users,
                d => d.UserId,
                u => u.Id,
                (d, u) => new { d.UserId, d.TotalTrees, u.FullName })
            .OrderByDescending(x => x.TotalTrees)
            .ToList();

        var topLeaders = allRankingsRaw
            .Take(2)
            .Select((x, i) => new HomeLeaderItemDto(
                Rank: i + 1,
                FullName: x.FullName,
                TreeCount: x.TotalTrees,
                IsCurrentUser: x.UserId == currentUserId))
            .ToList();

        var userEntry = allRankingsRaw
            .Select((x, i) => new { x.UserId, x.TotalTrees, Rank = i + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var currentUserRank = userEntry is not null
            ? new HomeUserRankDto(
                Rank: userEntry.Rank,
                TreeCount: userEntry.TotalTrees,
                Message: $"{userEntry.TotalTrees} Ağaç ile {userEntry.Rank}. sıradasınız.")
            : null;

        // 6. Progress hesapla
        var globalProgress = globalTarget > 0
            ? Math.Min(100, Math.Round((double)totalDonatedAllTime / globalTarget * 100, 1))
            : 0;

        var monthlyProgress = monthlyGoal > 0
            ? Math.Min(100, Math.Round((double)totalDonatedThisMonth / monthlyGoal * 100, 1))
            : 0;

        return Result<GetHomePageResponse>.Success(new GetHomePageResponse(
            HasCompletedPoll: true,
            GlobalTarget: new GlobalTargetDto(
                TargetTreeCount: globalTarget,
                DonatedTreeCount: totalDonatedAllTime,
                RemainingTreeCount: Math.Max(0, globalTarget - totalDonatedAllTime),
                ProgressPercent: globalProgress),
            MonthlyTarget: new MonthlyTargetDto(
                Month: now.Month,
                Year: now.Year,
                TargetTreeCount: monthlyGoal,
                DonatedTreeCount: totalDonatedThisMonth,
                RemainingTreeCount: Math.Max(0, monthlyGoal - totalDonatedThisMonth),
                ProgressPercent: monthlyProgress),
            TopLeaders: topLeaders,
            CurrentUserRank: currentUserRank));
    }
}