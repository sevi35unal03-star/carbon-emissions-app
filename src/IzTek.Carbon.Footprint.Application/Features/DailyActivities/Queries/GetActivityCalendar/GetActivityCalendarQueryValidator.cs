namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetActivityCalendar
{
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

      }
    }

}
