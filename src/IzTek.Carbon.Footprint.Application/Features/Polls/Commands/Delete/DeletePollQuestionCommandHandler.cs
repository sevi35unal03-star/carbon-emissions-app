
using IzTek.Carbon.Footprint.Domain.Events.Poll;


namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public static class DeletePollQuestionCommandHandler
{
    public static async Task<Result> Handle(
    DeletePollQuestionCommand command,
    IApplicationDbContext context,
    IMessageBus bus,
    CancellationToken ct)
    {
        var question = await context.PollQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == command.QuestionId, ct);

        if (question == null)
            return Result.Failure(
                SystemErrorCodes.PollQuestionNotFound, HttpStatusCode.NotFound);

        context.PollQuestions.Remove(question);

        if (await context.SaveChangesAsync(ct) <= 0)
            return Result.Failure(
                SystemErrorCodes.PollQuestionDeleteFailed, HttpStatusCode.InternalServerError);

        await bus.PublishAsync(new PollQuestionDeletedDomainEvent(command.QuestionId), ct);

        return Result.NoContent();
    }
}