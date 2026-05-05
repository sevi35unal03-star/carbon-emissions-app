namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;

public record GetActivityCalendarQuery(
    int Year,
    int? Month = null);


public class CalendarResponse
{
    public double TotalScore { get; set; } // Seçilen yıla veya aya ait toplam skor
    public List<CalendarItemDto> Items { get; set; } = new();
};

public class CalendarItemDto
{
    public DateTime Date { get; set; }

    public double Score { get; set; }

    public bool HasDetails { get; set; }
};