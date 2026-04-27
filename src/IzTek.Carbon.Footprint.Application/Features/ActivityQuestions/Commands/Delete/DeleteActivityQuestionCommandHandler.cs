using IzTek.Carbon.Footprint.Domain.Common.Exceptions;
using IzTek.Carbon.Footprint.Domain.Events.Activity;

namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Delete;

public static class DeleteActivityQuestionHandler
{
    // Wolverine bu metodu parametre tipinden (DeleteActivityQuestionCommand) otomatik tanır.
    public static async Task<Result> Handle(
        DeleteActivityQuestionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var question = await context.ActivityQuestions
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (question is null)
            throw new DomainException(SystemErrorCodes.ActivityQuestionNotFound);

        context.ActivityQuestions.Remove(question);

        question.AddDomainEvent(
            new ActivityQuestionDeletedDomainEvent(command.Id));

        await context.SaveChangesAsync(ct);

        return Result.Success();
    }
}