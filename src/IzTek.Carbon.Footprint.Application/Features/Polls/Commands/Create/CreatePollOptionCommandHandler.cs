using IzTek.Carbon.Footprint.Application.Common.Interfaces;
using IzTek.Carbon.Footprint.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollOptionCommandHandler
{
    public async Task<Result<Guid>> HandleAsync(
        CreatePollOptionCommand command,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        // 1. Question var mı kontrol et
        var question = await context.PollQuestions
            .FirstOrDefaultAsync(x => x.Id == command.QuestionId, ct); 

        if (question is null)
            return Result<Guid>.Failure("Soru bulunamadı.", HttpStatusCode.NotFound);

        // 2. PollOption oluştur
        var option = new PollOption(
            pollQuestionId: command.QuestionId,  
            text: command.Text,
            carbonValue: command.Value,
            nextPollQuestionId: null,
            displayOrder: command.DisplayOrder
        );

        context.PollOptions.Add(option);
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Success(option.Id);
    }
}