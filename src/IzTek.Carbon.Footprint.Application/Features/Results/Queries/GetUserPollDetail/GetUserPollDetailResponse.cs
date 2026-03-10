namespace IzTek.Carbon.Footprint.Application.Features.Results.Queries.GetUserPollDetail;

public class PollAnswerDetailDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string SelectedOptionText { get; set; } = string.Empty;
    public double CarbonValue { get; set; }
}


public class UserPollDetailResponse
{
    public UserPollDetailResponse(string userName, double totalScore, int treeCount, List<PollAnswerDetailDto> answers)
    {
        UserName = userName;
        TotalScore = totalScore;
        TreeCount = treeCount;
        Answers = answers;
    }

    public string UserName { get; set; } = string.Empty;
    public double TotalScore { get; set; }
    public int TreeCount { get; set; }
    public List<PollAnswerDetailDto> Answers { get; set; }
}