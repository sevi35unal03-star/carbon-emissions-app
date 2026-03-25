namespace IzTek.Carbon.Footprint.Domain.Entities;

public class PollOption : BaseAuditableEntity
{
    public Guid PollQuestionId { get; private set; }
    public string Text { get; private set; } = null!;
    public string? Message { get; private set; } // ← seçenek altı açıklama mesajı
    public double CarbonValue { get; private set; }
    public Guid? NextPollQuestionId { get; private set; }
    public int DisplayOrder { get; private set; }

    public PollQuestion PollQuestion { get; private set; } = null!;

    private PollOption()
    { }

    public PollOption(
        Guid pollQuestionId,
        string text,
        double carbonValue,
        string? message = null,
        Guid? nextPollQuestionId = null,
        int displayOrder = 0)
    {
        PollQuestionId = pollQuestionId;
        Text = text;
        CarbonValue = carbonValue;
        Message = message;
        NextPollQuestionId = nextPollQuestionId;
        DisplayOrder = displayOrder;
    }

    public static PollOption CloneFrom(ActivityOption source, Guid pollQuestionId)
    {
        return new PollOption(
            pollQuestionId: pollQuestionId,
            text: source.Text,
            carbonValue: source.CarbonValue,
            message: null, // ← aktivite sorularında mesaj yok
            nextPollQuestionId: null,
            displayOrder: source.DisplayOrder
        );
    }

    public void UpdateDetails(
        string text,
        double carbonValue,
        string? message,
        Guid? nextPollQuestionId,
        int displayOrder)
    {
        Text = text;
        CarbonValue = carbonValue;
        Message = message;
        NextPollQuestionId = nextPollQuestionId;
        DisplayOrder = displayOrder;
    }
}