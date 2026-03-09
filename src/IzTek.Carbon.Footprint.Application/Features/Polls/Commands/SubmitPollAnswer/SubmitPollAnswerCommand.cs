namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public record PollAnswerItem(Guid QuestionId, Guid OptionId);

public record SubmitPollAnswerCommand(
    Guid PollSetId,
    List<PollAnswerItem> Answers) : ICacheInvalidator
{
    public IEnumerable<string> CacheKeys =>
        [$"monthly-leaderboard:{DateTime.Now.Month}:{DateTime.Now.Year}",
         $"goal-detail:{DateTime.Now.Month}:{DateTime.Now.Year}"];
}


//SubmitPollAnswer Validator ekle

public class SubmitPollAnswerValidator : AbstractValidator<SubmitPollAnswerCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SubmitPollAnswerValidator(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;

        // PollSetId için özel bir kural tanımlıyoruz
        RuleFor(x => x.PollSetId)
            .MustAsync(BeFirstTimeThisMonth)
                .WithMessage("Bu anketi bu ay zaten cevapladınız. Yeni ayda tekrar deneyebilirsiniz.");

        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("Anket cevapları boş olamaz.");
    }

    private async Task<bool> BeFirstTimeThisMonth(Guid pollSetId, CancellationToken ct)
    {
        // 1. İçinde bulunduğumuz ayı ve yılı al
        int currentMonth = DateTime.UtcNow.Month;
        int currentYear = DateTime.UtcNow.Year;

        // 2. Yeni oluşturduğumuz UserPollResults tablosuna bak
        // Bu kullanıcı, bu ay, bu anket setini çözmüş mü?
        bool alreadyAnswered = await _context.UserPollResults
            .AnyAsync(x => x.UserId == _currentUser.UserId &&
                           x.PollSetId == pollSetId &&
                           x.Month == currentMonth &&
                           x.Year == currentYear, ct);

        // Eğer cevaplanmamışsa (false ise) kuraldan geçer (true döner)
        return !alreadyAnswered;
    }
}