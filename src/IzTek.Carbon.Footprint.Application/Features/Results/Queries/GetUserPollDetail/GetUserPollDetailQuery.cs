namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public record GetUserPollDetailQuery
{
    public Guid? TargetUserId { get; init; } // Sadece Admin kullanır, opsiyonel — normal kullanıcı göndermez
    public int Month { get; init; }
    public int Year { get; init; }
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