using AppCacheKeys = IzTek.Carbon.Footprint.Application.Common.Constants.CacheKeys;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public record PollAnswerItem(Guid QuestionId, Guid OptionId);

public record SubmitPollAnswerCommand(
    Guid PollSetId,
    List<PollAnswerItem> Answers) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
    [
        AppCacheKeys.Leaderboard.Monthly(DateTime.UtcNow.Month, DateTime.UtcNow.Year),
        AppCacheKeys.Goals.Detail(DateTime.UtcNow.Month, DateTime.UtcNow.Year)
    ];
}

public class SubmitPollAnswerValidator : AbstractValidator<SubmitPollAnswerCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SubmitPollAnswerValidator(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        RuleFor(x => x.PollSetId)
            .MustAsync(BeFirstTimeThisMonth)
            .WithMessage("Bu anketi bu ay zaten cevapladınız. Yeni ayda tekrar deneyebilirsiniz.");

        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("Anket cevapları boş olamaz.");
    }

    private async Task<bool> BeFirstTimeThisMonth(Guid pollSetId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var alreadyAnswered = await _context.UserPollResults
            .AnyAsync(x => x.UserId == _currentUser.UserId &&
                           x.PollSetId == pollSetId &&
                           x.Month == now.Month &&
                           x.Year == now.Year, ct);
        return !alreadyAnswered;
    }
}