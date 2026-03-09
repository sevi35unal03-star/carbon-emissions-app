namespace IzTek.Carbon.Footprint.Application.Features.Polls.Commands.Create;

public class CreatePollSetRequest
{
    public string Name { get; set; } // Örn: "2024 Ocak Ayı Anketi"
    public int Month { get; set; }
    public int Year { get; set; }

    // Anket setine ait sorular listesi
    public List<CreatePollQuestionRequest> Questions { get; set; } = new();
};