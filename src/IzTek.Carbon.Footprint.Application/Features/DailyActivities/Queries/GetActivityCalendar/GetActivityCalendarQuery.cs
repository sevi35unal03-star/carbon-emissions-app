namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar;

public record GetActivityCalendarQuery(
    int Year,
    int? Month = null,
    int Period = 1);

public class GetActivityCalendarQueryValidator : AbstractValidator<GetActivityCalendarQuery>
{
    public GetActivityCalendarQueryValidator()
    {
        RuleFor(x => x.Year)
            .InclusiveBetween(2020, 2100)
            .WithMessage("Geçerli bir yıl giriniz.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12)
            .When(x => x.Month.HasValue)
            .WithMessage("Geçerli bir ay giriniz. (1-12)");

        RuleFor(x => x.Period)
            .InclusiveBetween(1, 2)
            .WithMessage("Period 1 veya 2 olmalıdır.");
    }
}

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