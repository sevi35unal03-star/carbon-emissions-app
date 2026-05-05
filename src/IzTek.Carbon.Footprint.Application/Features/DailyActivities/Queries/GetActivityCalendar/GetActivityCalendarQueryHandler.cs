using Microsoft.EntityFrameworkCore;

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

        var now = DateTime.UtcNow;
        var lastDay = DateTime.DaysInMonth(now.Year, now.Month);

        var startDate = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(now.Year, now.Month, lastDay, 23, 59, 59, DateTimeKind.Utc);

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