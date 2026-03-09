using Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;
using Iztek.Carbon.Footprint.Application.Features.Users.Queries.GetUserProfile;
using Spectre.Console;
namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;

public class GetDailyPollQueryHandler
{
    public async Task<Result<GetDailyPollResponse>> HandleAsync(
        IApplicationDbContext context,
        ICacheService cacheService,
        CancellationToken ct)
    {
        var cacheKey = "active_daily_poll";

        var cachedPoll = await cacheService.GetAsync<GetDailyPollResponse>(cacheKey);
        if (cachedPoll != null)
            return Result.Success(cachedPoll);

        var response = await context.PollSets
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new GetDailyPollResponse
            {
                PollSetId = x.Id,
                Name = x.Name,
                Description = x.Description,
                Questions = x.Questions
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
            })
            .FirstOrDefaultAsync(ct);
        if (response == null)
            return Result<GetDailyPollResponse>.Failure(
                "Şu anda aktif bir anket bulunmamaktadır.",
                HttpStatusCode.NotFound);

        await cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1));
        return Result.Success(response);
    }
}