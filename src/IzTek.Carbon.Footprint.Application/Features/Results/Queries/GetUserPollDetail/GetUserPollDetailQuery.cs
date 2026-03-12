namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public record GetUserPollDetailQuery
{
    public Guid PollSetId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public Guid? TargetUserId { get; set; }
}

public record PollAnswerDetailDto
{
    public string QuestionText { get; init; } = string.Empty;
    public string SelectedOptionText { get; init; } = string.Empty;
    public double CarbonValue { get; init; }
}

public record UserPollDetailResponse(
    string UserName,
    double TotalScore,
    int TreeCount,
    List<PollAnswerDetailDto> Answers);