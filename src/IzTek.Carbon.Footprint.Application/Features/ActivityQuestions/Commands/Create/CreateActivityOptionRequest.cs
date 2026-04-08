namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Create;

public class CreateActivityOptionRequest
{
    public string Text { get; set; } = default!;
    public double CarbonValue { get; set; }
    public Guid? NextQuestionId { get; set; }
}

