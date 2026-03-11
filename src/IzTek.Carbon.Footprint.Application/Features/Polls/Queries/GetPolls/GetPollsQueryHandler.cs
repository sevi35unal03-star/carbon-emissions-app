using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;


namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetPolls;

// ─────────────────────────────────────────────────────────────
// GET /polls  →  Tüm anket setlerini listeler  [Admin]
// ─────────────────────────────────────────────────────────────



public static class GetPollsQueryHandler
{
    public static async Task<Result<List<PollSummaryResponse>>> Handle(
        GetPollsQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var polls = await context.PollSets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new PollSummaryResponse(
                Id: x.Id,
                Name: x.Name,
                Description: x.Description,
                IsActive: x.IsActive,
                CreatedAt: x.CreatedAt,
                QuestionCount: x.Questions.Count))
            .ToListAsync(ct);

        return Result<List<PollSummaryResponse>>.Success(polls);
    }
}

// ─────────────────────────────────────────────────────────────
// GET /polls/{id}  →  Anket detayı (sorular ve seçenekler)  [Admin]
// ─────────────────────────────────────────────────────────────

public record GetPollByIdQuery(Guid Id);

public record PollDetailResponse(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    DateTime CreatedAt,
    List<PollQuestionResponse> Questions);

public static class GetPollByIdQueryHandler
{
    public static async Task<Result<PollDetailResponse>> Handle(
        GetPollByIdQuery query,
        IApplicationDbContext context,
        CancellationToken ct)
    {
        var poll = await context.PollSets
            .AsNoTracking()
            .Include(x => x.Questions)
                .ThenInclude(q => q.Options)
            .Where(x => x.Id == query.Id)
            .Select(x => new PollDetailResponse(
                Id: x.Id,
                Name: x.Name,
                Description: x.Description,
                IsActive: x.IsActive,
                CreatedAt: x.CreatedAt,
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
                    .ToList()))
            .FirstOrDefaultAsync(ct);

        if (poll is null)
            return Result<PollDetailResponse>.Failure(
                SystemErrorCodes.ActivePollNotFound, HttpStatusCode.NotFound);

        return Result<PollDetailResponse>.Success(poll);
    }
}