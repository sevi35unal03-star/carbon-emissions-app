namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollQuestionCommandHandler
{
    public async Task<Result<Guid>> HandleAsync(
        CreatePollQuestionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var exists = await context.PollSets
    .AnyAsync(x => x.Id == command.PollSetId, ct);

        if (!exists)
            return Result<Guid>.Failure("PollSet bulunamadı.");

        // 2. PollQuestion oluştur
        var question = new PollQuestion(
            pollSetId: command.PollSetId,  
            text: command.Text,
            displayOrder: command.DisplayOrder
        );

        context.PollQuestions.Add(question);
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Success(question.Id); 
    }
}