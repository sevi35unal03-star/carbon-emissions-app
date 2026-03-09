namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;

public record GetActivityCalendarQuery(
    int Year,
    int? Month,
    int Period = 1) : ICacheableQuery
{
    public string CacheKey => Month.HasValue
        ? $"activity-calendar:{Year}:{Month}"
        : $"activity-calendar:{Year}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(15);
}

public class CalendarResponse
{
    public int TotalScore { get; set; } // Seçilen yıla veya aya ait toplam skor
    public List<CalendarItemDto> Items { get; set; } = new();
};

public class CalendarItemDto
{
    public DateTime Date { get; set; }

    public int Score { get; set; }

    public bool HasDetails { get; set; }
};

