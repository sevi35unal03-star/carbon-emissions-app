namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;

public record GetActivityCalendarQuery(
    int Year,
    int? Month, // Null ise yıllık görünüm, dolu ise aylık görünüm
    int Period = 1);

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

