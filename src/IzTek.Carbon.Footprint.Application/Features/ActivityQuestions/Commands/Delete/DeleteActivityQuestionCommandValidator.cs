namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Delete
{
    public class DeleteActivityQuestionCommandValidator : AbstractValidator<DeleteActivityQuestionCommand>
    {
        public DeleteActivityQuestionCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Question ID is required.");
        }
    }
}
