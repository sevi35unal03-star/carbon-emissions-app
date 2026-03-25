namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.SubmitPollAnswer;

public class SubmitPollAnswerResponse
{
    public SubmitPollAnswerResponse(double totalCarbonScore, int calculatedTrees)
    {
        TotalCarbonScore = totalCarbonScore;
        CalculatedTrees = calculatedTrees;
    }

    public double TotalCarbonScore { get; set; }
    public int CalculatedTrees { get; }
};