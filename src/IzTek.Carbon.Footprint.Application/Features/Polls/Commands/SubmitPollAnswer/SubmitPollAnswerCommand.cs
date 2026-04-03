namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public record PollAnswerItem(Guid QuestionId, Guid OptionId);

public record SubmitPollAnswerCommand(
    Guid PollSetId,
    List<PollAnswerItem> Answers,
    bool IsDraft = false); // ← yeni

public class SubmitPollAnswerValidator : AbstractValidator<SubmitPollAnswerCommand>
{
    public SubmitPollAnswerValidator()
    {
        RuleFor(x => x.PollSetId)
            .NotEmpty().WithMessage("Anket ID boş olamaz.");

        RuleFor(x => x.Answers)
            .NotEmpty().WithMessage("Anket cevapları boş olamaz.");
    }
}