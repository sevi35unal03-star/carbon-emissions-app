namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries.GetById;

public static class GetActivityQuestionByIdQueryHandler
{
    public static async Task<Result<ActivityQuestionResponse>> Handle(
        GetActivityQuestionByIdQuery request,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var question = await context.ActivityQuestions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (question == null)
            return Result<ActivityQuestionResponse>.Failure(SystemErrorCodes.ActivityQuestionNotFound, HttpStatusCode.NotFound);

        var response = question.Adapt<ActivityQuestionResponse>();

        return Result<ActivityQuestionResponse>.Success(response);
    }
}