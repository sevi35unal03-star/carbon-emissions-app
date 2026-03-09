using Iztek.Carbon.Footprint.Domain.Entities;

namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollQuestion : BaseAuditableEntity
{
    public Guid PollSetId { get; private set; }
    public string Text { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    // Navigation property
    public PollSet PollSet { get; private set; } = null!;

    private readonly List<PollOption> _options = new();
    public IReadOnlyCollection<PollOption> Options => _options.AsReadOnly();

    private PollQuestion() { }

    public PollQuestion(Guid pollSetId, string text, int displayOrder)
    {
        PollSetId = pollSetId;
        Text = text;
        DisplayOrder = displayOrder;
    }

    // ✅ CloneFrom düzeltildi
    public static PollQuestion CloneFrom(ActivityQuestion source, Guid pollSetId)
    {
        var question = new PollQuestion(
            pollSetId: pollSetId,
            text: source.Text,
            displayOrder: source.DisplayOrder
        );

        // ✅ question.Id kullanıldı
        foreach (var option in source.Options)
            question._options.Add(PollOption.CloneFrom(option, question.Id));

        return question;
    }

    public void AddOption(string text, double carbonValue, Guid? nextQuestionId, int displayOrder)
    {
        // ✅ Id kullanıldı
        _options.Add(new PollOption(Id, text, carbonValue, nextQuestionId, displayOrder));
    }

    public void UpdateDetails(string text, int displayOrder)
    {
        Text = text;
        DisplayOrder = displayOrder;
    }
}