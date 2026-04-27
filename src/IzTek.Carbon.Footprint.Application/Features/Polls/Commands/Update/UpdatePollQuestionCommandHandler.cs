namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Update;

public class UpdatePollQuestionCommandHandler
{
    public async Task<Result> Handle(
        UpdatePollQuestionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Question'ı getir
        var question = await context.PollQuestions
            .FirstOrDefaultAsync(x => x.Id == command.QuestionId, ct); // ✅ PollQuestionId → Id

        if (question is null)
            return Result.Failure(SystemErrorCodes.PollQuestionNotFound, HttpStatusCode.NotFound); // ✅ throw yerine Result.Failure

        // 2. Domain metodu ile güncelle
        question.UpdateDetails(
            text: command.Text,
            displayOrder: command.DisplayOrder); // ✅ Direkt property atama yerine domain metodu

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}