

namespace IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetMonthlyPoll;
public class PollQuestionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // Options listesini sınıfın içine taşıdık ve null hatası almamak için initialize ettik.
    public List<PollOptionResponse> Options { get; set; } = new();
}