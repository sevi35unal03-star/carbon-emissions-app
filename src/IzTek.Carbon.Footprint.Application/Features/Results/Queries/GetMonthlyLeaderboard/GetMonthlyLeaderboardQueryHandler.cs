namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetMonthlyLeaderboard;

public static class GetMonthlyLeaderboardQueryHandler
{
    public static async Task<Result<GetMonthlyLeaderboardResponse>> Handle(
        GetMonthlyLeaderboardQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        CancellationToken ct)
    {
        // Not 2 düzeltmesi: MONTH/YEAR yerine range sorgu → index kullanır
        var start = new DateTime(query.Year, query.Month, 1);
        var end = start.AddMonths(1);

        var rankingsRaw = await (
            from donation in context.TreeDonations
            where donation.DonationDate >= start && donation.DonationDate < end
            join user in context.Users on donation.UserId equals user.Id
            group new { donation, user } by new { donation.UserId, user.Name, user.Surname } into g
            select new
            {
                UserId = g.Key.UserId,
                FullName = g.Key.Name + " " + g.Key.Surname,
                TotalTrees = g.Sum(x => x.donation.TreeCount)
            }
        )
        .OrderByDescending(x => x.TotalTrees)
        .ToListAsync(ct);
        var currentUserId = currentUser.UserId;

        var podium = rankingsRaw
            .Take(3)
            .Select((x, i) => new PodiumItemDto(i + 1, x.FullName, x.TotalTrees, x.UserId == currentUserId))
            .ToList();

        var leaders = rankingsRaw
            .Skip(3)
            .Take(7) // 3. kişiden sonraki 7 kişi
            //index 4 ten başlasın
            .Select((x, i) => new LeaderboardItemDto(i + 4, x.FullName, x.TotalTrees, x.UserId == currentUserId))
            .ToList();

        var userEntry = rankingsRaw
            .Select((x, i) => new { x.UserId, x.TotalTrees, Rank = i + 1 })
            .FirstOrDefault(x => x.UserId == currentUserId);

        var userRankDto = userEntry is not null
            ? new UserRankDto(userEntry.Rank, userEntry.TotalTrees,
                $"{userEntry.TotalTrees} Ağaç ile {userEntry.Rank}. sıradasınız.")
            : null;

        return Result<GetMonthlyLeaderboardResponse>.Success(new GetMonthlyLeaderboardResponse(
            Podium: podium,
            Leaders: leaders,
            CurrentUserRank: userRankDto));
    }
}