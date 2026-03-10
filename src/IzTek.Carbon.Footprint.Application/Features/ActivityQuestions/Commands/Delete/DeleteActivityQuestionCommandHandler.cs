using IzTek.Carbon.Footprint.Application.Common.Models;
using IzTek.Carbon.Footprint.Domain.Entities;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Delete;

public static class DeleteActivityQuestionHandler
{
    // Wolverine bu metodu parametre tipinden (DeleteActivityQuestionCommand) otomatik tanır.
    public static async Task<Result> Handle(
        DeleteActivityQuestionCommand command,
        IApplicationDbContext context)
    {
        var question = await context.ActivityQuestions
            .FirstOrDefaultAsync(x => x.PollQuestionId == command.Id);

        if (question == null)
            return Result.Failure(
                SystemErrorCodes.ActivityQuestionNotFound, HttpStatusCode.NotFound);

        context.ActivityQuestions.Remove(question);

        question.AddDomainEvent(new ActivityQuestionDeletedDomainEvent(command.Id));

        await context.SaveChangesAsync(default);

        return Result.Success();
    }
}