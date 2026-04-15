using IzTek.Carbon.Footprint.Application.Common;
using IzTek.Carbon.Footprint.Application.Features.Goals.Commands;
using Microsoft.EntityFrameworkCore;

namespace IzTek.Carbon.Footprint.Application.Features.Goals.Queries;

public class GetGlobalGoalsQueryHandler
{
    private readonly IApplicationDbContext _context;

    public GetGlobalGoalsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<GlobalGoalResponse>>> Handle(GetGlobalGoalsQuery request)
    {
        var goals = await _context.Goals
            .Where(x => x.IsActive)
            .Select(x => new GlobalGoalResponse(
                x.Id,
                x.Month,
                x.Year,
                x.TargetTreeCount
            ))
            .ToListAsync();

        return Result<List<GlobalGoalResponse>>.Success(goals);
    }
}