namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public class PollAnswerDetailDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string SelectedOptionText { get; set; } = string.Empty;
    public double ScoreAtTime { get; set; }
}

public class UserPollDetailResponse
{
    public UserPollDetailResponse(object userName, double totalScore, int treeCount, object answers)
    {
        UserName1 = userName;
        TotalScore = totalScore;
        TreeCount = treeCount;
        Answers1 = answers;
    }

    public string UserName { get; set; } = string.Empty;
    public double TotalScore { get; set; }
    public int TreeCount { get; set; }

    public List<PollAnswerDetailDto> Answers { get; set; } = new();
    public object UserName1 { get; }
    public object Answers1 { get; }
}