using IzTek.Carbon.Footprint.Application.Features.DailyActivities.Queries.GetDailyQuestions;

namespace IzTek.Carbon.Footprint.Application.Features.DailyActivities.Commands.SubmitAnswer;

public class SubmitActivityAnswerResponse
{
    public double TotalCarbonScore { get; set; }
    public bool IsFlowCompleted { get; set; }
    public List<AnswerSummaryDto> Answers { get; set; } = new();
}

public record AnswerSummaryDto(
    string QuestionText,
    string SelectedOptionText,
    double CarbonValue
);