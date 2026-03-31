namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public record PollAnswerItem(Guid QuestionId, Guid OptionId);

public record SubmitPollAnswerCommand(
    Guid PollSetId,
    List<PollAnswerItem> Answers,
    bool IsDraft = false); // ← yeni

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

    private async Task<bool> BeFirstTimeThisMonth(
        SubmitPollAnswerCommand command, // ← command'a erişmek için
        Guid pollSetId,
        ValidationContext<SubmitPollAnswerCommand> context,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // Sadece tamamlanmış anketi kontrol et — taslak engel değil
        var alreadyCompleted = await _context.UserPollResults
            .AnyAsync(x => x.UserId == _currentUser.UserId &&
                           x.PollSetId == pollSetId &&
                           x.Month == now.Month &&
                           x.Year == now.Year &&
                           x.IsCompleted, ct); // ← IsCompleted eklendi

        return !alreadyCompleted;
    }
}