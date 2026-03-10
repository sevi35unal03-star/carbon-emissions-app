using Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;
using Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using Spectre.Console;
namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;

public class GetMonthlyPollQueryHandler
{
    public async Task<Result<GetMonthlyPollResponse>> HandleAsync(
        IApplicationDbContext context,
        ICacheService cacheService,
        CancellationToken ct)
    {
        var cacheKey = "active_daily_poll";

        var cachedPoll = await cacheService.GetAsync<GetMonthlyPollResponse>(cacheKey);
        if (cachedPoll != null)
            return Result<GetMonthlyPollResponse>.Success(cachedPoll);

        var response = await context.PollSets
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetMonthlyPollResponse(
                PollSetId: x.Id,
                Name: x.Name,
                Description: x.Description,
                Questions: x.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new PollQuestionResponse
                    {
                        Id = q.Id,
                        Text = q.Text,
                        DisplayOrder = q.DisplayOrder,
                        Options = q.Options
                            .Select(o => new PollOptionResponse
                            {
                                Id = o.Id,
                                Text = o.Text
                            })
                            .ToList()
                    })
                    .ToList()
            ))
                        .FirstOrDefaultAsync(ct);

        if (response == null)
            return Result<GetMonthlyPollResponse>.Failure(
                SystemErrorCodes.ActivePollNotFound, HttpStatusCode.NotFound);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1));
        return Result<GetMonthlyPollResponse>.Success(response);
    }
}