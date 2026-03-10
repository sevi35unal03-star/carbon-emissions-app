namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public record GetUserPollDetailQuery
{
    public Guid UserId { get; internal set; }
    public int Month { get; internal set; }
    public int Year { get; internal set; }
    public string UserName { get; internal set; } = string.Empty;
}