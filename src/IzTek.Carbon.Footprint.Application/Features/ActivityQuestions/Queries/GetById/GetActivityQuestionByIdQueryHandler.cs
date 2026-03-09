using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;
using IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetById;

namespace Iztek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetById;

public static class GetActivityQuestionByIdQueryHandler
{
    public static async Task<Result<ActivityQuestionResponse>> HandleAsync(
        GetActivityQuestionByIdQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var question = await context.ActivityQuestions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.PollQuestionId == request.Id, ct);

        if (question == null)
            return Result.Failure<ActivityQuestionResponse>("Question is not found.");

        var response = question.Adapt<ActivityQuestionResponse>();

        return Result.Success(response);
    }
}