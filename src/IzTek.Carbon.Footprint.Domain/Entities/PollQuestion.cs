namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollQuestion : BaseAuditableEntity
{
    public Guid PollSetId { get; private set; }
    public string Text { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    // Navigation property
    public PollSet PollSet { get; private set; } = null!;

    private readonly List<PollOption> _options = [];
    public IReadOnlyCollection<PollOption> Options => _options.AsReadOnly();

    private PollQuestion()
    { }

    public PollQuestion(Guid pollSetId, string text, int displayOrder)
    {
        PollSetId = pollSetId;
        Text = text;
        DisplayOrder = displayOrder;
    }

    public static PollQuestion CloneFrom(ActivityQuestion source, Guid pollSetId)
    {
        var question = new PollQuestion(
            pollSetId: pollSetId,
            text: source.Text,
            displayOrder: source.DisplayOrder
        );

        foreach (var option in source.Options)
            question._options.Add(PollOption.CloneFrom(option, question.Id));

        return question;
    }

    public void AddOption(string text, double carbonValue, string? message = null, Guid? nextQuestionId = null, int displayOrder = 0)
    {
        _options.Add(new PollOption(
            pollQuestionId: Id,
            text: text,
            carbonValue: carbonValue,
            message: message,
            nextPollQuestionId: nextQuestionId,
            displayOrder: displayOrder));
    }

    public void UpdateDetails(string text, int displayOrder)
    {
        Text = text;
        DisplayOrder = displayOrder;
    }
}