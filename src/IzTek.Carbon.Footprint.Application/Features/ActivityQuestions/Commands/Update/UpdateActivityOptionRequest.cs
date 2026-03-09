namespace IzTek.Carbon.Footprint.Application.Features.ActivityQuestions.Commands.Update;

public class UpdateActivityOptionRequest
{
    public Guid? Id { get; set; } 
    public string Text { get; set; } = default!;
    public double CarbonValue { get; set; }
    public Guid? NextQuestionId { get; set; }
}