namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer
{
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
}
