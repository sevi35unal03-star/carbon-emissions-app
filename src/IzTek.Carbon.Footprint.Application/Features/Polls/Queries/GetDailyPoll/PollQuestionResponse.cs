using IzTek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;

namespace Iztek.Carbon.Footprint.Application.Features.Polls.Queries.GetDailyPoll;
public class PollQuestionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public int DisplayOrder { get; set; }

    // Options listesini sınıfın içine taşıdık ve null hatası almamak için initialize ettik.
    public List<PollOptionResponse> Options { get; set; } = new();
}