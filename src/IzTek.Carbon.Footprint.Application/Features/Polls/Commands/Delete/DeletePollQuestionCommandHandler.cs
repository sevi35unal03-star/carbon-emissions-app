using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Application.Features.Polls.Events;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Delete;

public class DeletePollQuestionCommandHandler
{
    public static async Task Handle(
        DeletePollQuestionCommand command,
        IApplicationDbContext context,
        IMessageBus bus,
        CancellationToken ct)
    {
        var question = await context.PollQuestions
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.QuestionId, ct);

        if (question == null)
            throw new Exception("Question not found.");

        context.PollQuestions.Remove(question);

        await context.SaveChangesAsync(ct);

        await bus.PublishAsync(new PollQuestionDeletedDomainEvent(command.QuestionId), ct);
    }
}