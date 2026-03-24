using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public record SubmitActivityAnswerCommand(
    Guid QuestionId,
    Guid SelectedOptionId,
    Guid UserId
) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.ActivityCalendar.UserPrefix(UserId)  // prefix ile tüm kullanıcı takvim cache'lerini temizle
    ];
}