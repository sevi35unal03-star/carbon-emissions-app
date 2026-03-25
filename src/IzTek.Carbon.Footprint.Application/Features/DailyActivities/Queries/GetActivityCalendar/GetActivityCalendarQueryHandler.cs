namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;

public static class GetActivityCalendarQueryHandler
{
    public static async Task<Result<CalendarResponse>> Handle(
        GetActivityCalendarQuery query,
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        CancellationToken ct)
    {
        var userId = currentUserService.UserId;

        DateTime startDate;
        DateTime endDate;

        if (query.Month.HasValue)
        {
            if (query.Period == 1)
            {
                startDate = new DateTime(query.Year, query.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
                endDate = new DateTime(query.Year, query.Month.Value, 15, 23, 59, 59, DateTimeKind.Utc);
            }
            else
            {
                startDate = new DateTime(query.Year, query.Month.Value, 16, 0, 0, 0, DateTimeKind.Utc);
                endDate = new DateTime(query.Year, query.Month.Value,
                    DateTime.DaysInMonth(query.Year, query.Month.Value), 23, 59, 59, DateTimeKind.Utc);
            }
        }
        else
        {
            startDate = new DateTime(query.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            endDate = new DateTime(query.Year, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        var items = await context.UserActivityAnswers
            .AsNoTracking()
            .Where(a => a.UserId == userId
                     && a.AnsweredAt >= startDate
                     && a.AnsweredAt <= endDate)
            .GroupBy(x => x.AnsweredAt.Date)
            .Select(g => new CalendarItemDto
            {
                Date = g.Key,
                Score = g.Sum(x => x.CarbonValue),
                HasDetails = true
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        return Result<CalendarResponse>.Success(new CalendarResponse
        {
            TotalScore = items.Sum(x => x.Score),
            Items = items
        });
    }
}