namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Queries;

public class ActivityOptionResponse
{
    public Guid Id { get; set; }

    public string Text { get; set; } = null!;   

    public double CarbonValue { get; set; }
    public Guid? NextQuestionId { get; set; }
};